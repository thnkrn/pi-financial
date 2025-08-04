using System.Net.Sockets;
using MassTransit;
using Microsoft.Extensions.Options;
using Pi.OnePort.TCP.API.Options;
using Pi.OnePort.TCP.API.Utils;
using Pi.OnePort.IntegrationEvents;
using Pi.OnePort.TCP.API.Factories;
using Pi.OnePort.TCP.Client;
using Pi.OnePort.TCP.Enums;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Exceptions;
using Pi.OnePort.TCP.Models;
using Pi.OnePort.TCP.Models.Packets;
using Pi.OnePort.TCP.Models.Packets.DataTransfer;

namespace Pi.OnePort.TCP.API.BackgroundServices;

public class OnePortListener : BackgroundService
{
    private readonly string _username;
    private readonly string _password;

    private readonly IBus _bus;
    private readonly ILogger<OnePortListener> _logger;

    private Task? _connectTask;
    private CancellationTokenSource? _keepAliveCancellationTokenSource;
    private int _packetSequence = -1;

    private const string KeepAliveMessage = "oneport-srv";
    private const int KeepAlivePeriod = 10 * 1000;
    private const int RetryPeriod = 15 * 1000;

    private readonly TimeOnly _operatingHoursOpeningTime;
    private readonly TimeOnly _operatingHoursClosingTime;

    private DateTime _operatingHoursOpeningDateTime;
    private DateTime _operatingHoursClosingDateTime;
    private readonly IOnePortResponseMapClient _onePortClient;

    public OnePortListener(
        ILogger<OnePortListener> logger,
        IOnePortResponseMapClient onePortClient,
        IOptions<OnePortOptions> onePortOptions,
        IOptions<OperationHoursOptions> operatingHourOptions,
        IBus bus
    )
    {
        _username = onePortOptions.Value.UserName ?? throw new InvalidOperationException();
        _password = onePortOptions.Value.Password ?? throw new InvalidOperationException();

        _operatingHoursOpeningTime = TimeOnly.Parse(operatingHourOptions.Value.Start);
        _operatingHoursClosingTime = TimeOnly.Parse(operatingHourOptions.Value.End);

        _onePortClient = onePortClient;
        _bus = bus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var now = DateUtils.GetThDateTimeNow();
            (_operatingHoursOpeningDateTime, _operatingHoursClosingDateTime) = OperatingHoursUtils
                .GetOperatingDateTime(
                    now,
                    _operatingHoursOpeningTime,
                    _operatingHoursClosingTime
                );

            if (now < _operatingHoursOpeningDateTime && now > _operatingHoursClosingDateTime)
            {
                var timeDiff = _operatingHoursOpeningDateTime - now;
                _logger.LogInformation("Will await for opening hour ({TimeDiff})", timeDiff);
                await Task.Delay(timeDiff, cancellationToken);
                (_operatingHoursOpeningDateTime, _operatingHoursClosingDateTime) = OperatingHoursUtils
                    .GetOperatingDateTime(
                        now,
                        _operatingHoursOpeningTime,
                        _operatingHoursClosingTime
                    );
            }

            try
            {
                await Connect(cancellationToken);
                KeepAlive();
                await Listen(cancellationToken);
            }
            catch (OutOfOperatingHoursException)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong");
            }
            finally
            {
                _connectTask = null;
                _keepAliveCancellationTokenSource?.Cancel();
            }
        }

        _logger.LogDebug("End");
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cleaning up before shutting down");
        _onePortClient.Disconnect().GetAwaiter();

        return base.StopAsync(cancellationToken);
    }

    private Task Connect(CancellationToken cancellationToken) => _connectTask ??= _Connect(cancellationToken);

    private async Task _Connect(CancellationToken cancellationToken)
    {
        _packetSequence = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            // opening time check should be handle by caller?
            if (DateUtils.GetThDateTimeNow() > _operatingHoursClosingDateTime)
            {
                _logger.LogInformation("Out of operating hours");
                throw new OutOfOperatingHoursException();
            }

            try
            {
                await _onePortClient.Connect(cancellationToken);
                _logger.LogDebug("Successfully connect to remote");
                await Logon();
                break;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while establishing connection, retrying in {RetryPeriod}ms", RetryPeriod);
                await Task.Delay(RetryPeriod, cancellationToken);
            }
        }
    }

    private async Task Listen(CancellationToken cancellationToken, bool continueOnError = true)
    {
        _logger.LogInformation("Start listening to oneport");
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_connectTask == null)
            {
                await Task.Delay(100, cancellationToken);
                _logger.LogTrace("Re-connecting");
                continue;
            }

            // opening time check should be handle by caller?
            if (DateUtils.GetThDateTimeNow() > _operatingHoursClosingDateTime)
            {
                _logger.LogInformation("Out of operating hours");
                throw new OutOfOperatingHoursException();
            }

            try
            {
                var packet = await _onePortClient.ListenWithWaiting(cancellationToken);

                if (packet?.Data is not PacketHeartbeat { RequestType: RequestType.Business, Reserve: KeepAliveMessage })
                {
                    _logger.LogInformation("Received packet: {Packet}", packet?.ToString());
                }

                if (packet != null)
                {
                    _packetSequence = packet.Sequence;

                    await HandlePacket(packet);
                }
            }
            catch (PacketDeserializationException e)
            {
                _logger.LogError(e, "Error Occurred when handling packets: {Message}", e.Message);

                if (!continueOnError) throw;
                // do nothing and continue
            }
            catch (Exception e) when (e is SocketException or ObjectDisposedException)
            {
                _logger.LogError(e, "Error Occurred when handling packets: {Message}", e.Message);

                // if (e is not ObjectDisposedException) _onePortClient.Dispose();
                _connectTask = null;
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Occurred when handling packets: {Message}", e.Message);

                if (!continueOnError) throw;
            }
        }

        _logger.LogInformation("Finished listening to oneport");
    }

    private async Task HandlePacket(Packet packet)
    {
        // TODO check trader account id
        // if (false) return;

        switch (packet.Data.PacketType)
        {
            case PacketType.DataTransfer:
                var data = (PacketDataTransfer)packet.Data;

                switch (data.DataTransferPacketContent.MessageType)
                {
                    case MessageType.NewOrderResponse6A:
                        var newOrderContent = (DataTransferNewOrderResponse6A)data.DataTransferPacketContent;
                        await _bus.Publish(
                            IntegrationEventFactory.NewOnePortBrokerOrderCreated(newOrderContent, packet.Timestamp!.Value),
                            SetContextMetaData<OnePortBrokerOrderCreated>(packet, newOrderContent.FisOrderId, newOrderContent.RefOrderId)
                        );
                        break;
                    case MessageType.OrderAcknowledgementResponse7K:
                        var orderAckContent = (DataTransferOrderAcknowledgementResponse7K)data.DataTransferPacketContent;
                        switch (orderAckContent.OrderStatus)
                        {
                            case OrderStatus.Rejected:
                                await _bus.Publish(
                                    IntegrationEventFactory.NewOnePortOrderRejected(orderAckContent, packet.Timestamp!.Value),
                                    SetContextMetaData<OnePortOrderRejected>(packet, orderAckContent.FisOrderId, orderAckContent.RefOrderId)
                                );
                                break;
                            case OrderStatus.Accepted:
                            case OrderStatus.Warning:
                            default:
                                break;
                        }

                        break;
                    case MessageType.ExecutionReportResponse7E:
                        var execReportContent = (DataTransferExecutionReportResponse7E)data.DataTransferPacketContent;

                        switch (execReportContent.ExecutionTransType)
                        {
                            case ExecutionTransType.New:
                                if (execReportContent.Volume > 0)
                                {
                                    await _bus.Publish(
                                        IntegrationEventFactory.NewOnePortOrderMatched(execReportContent, packet.Timestamp!.Value),
                                        SetContextMetaData<OnePortOrderMatched>(packet, execReportContent.FisOrderId, execReportContent.RefOrderId)
                                    );
                                }
                                break;
                            case ExecutionTransType.Cancel:
                                await _bus.Publish(
                                    IntegrationEventFactory.NewOnePortOrderCancel(execReportContent, packet.Timestamp!.Value),
                                    SetContextMetaData<OnePortOrderCanceled>(packet, execReportContent.FisOrderId, execReportContent.RefOrderId)
                                );
                                break;
                            case ExecutionTransType.ChangeAcct:
                            case ExecutionTransType.Reject:
                            default:
                                break;
                        }

                        break;
                    case MessageType.OrderChangeResponse7N:
                        var orderChangedContent = (DataTransferOrderChangeResponse7N)data.DataTransferPacketContent;
                        switch (orderChangedContent.OrderStatus)
                        {
                            case OrderStatus.Rejected:
                                await _bus.Publish(
                                    IntegrationEventFactory.NewOnePortOrderRejected(orderChangedContent, packet.Timestamp!.Value),
                                    SetContextMetaData<OnePortOrderRejected>(packet, orderChangedContent.FisOrderId, orderChangedContent.RefOrderId)
                                );
                                break;
                            case OrderStatus.Accepted:
                            case OrderStatus.Warning:
                            default:
                                await _bus.Publish(
                                    IntegrationEventFactory.NewOnePortOrderChanged(orderChangedContent, packet.Timestamp!.Value),
                                    SetContextMetaData<OnePortOrderChanged>(packet, orderChangedContent.FisOrderId, orderChangedContent.RefOrderId)
                                );
                                break;
                        }

                        break;
                    case MessageType.OrderChangeByBrokerResponse6T:
                        var orderChangedByBrokerContent = (DataTransferOrderChangeByBrokerResponse6T)data.DataTransferPacketContent;
                        await _bus.Publish(
                            IntegrationEventFactory.NewOnePortOrderChanged(orderChangedByBrokerContent, packet.Timestamp!.Value),
                            SetContextMetaData<OnePortOrderChanged>(packet, orderChangedByBrokerContent.FisOrderId, orderChangedByBrokerContent.RefOrderId)
                        );
                        break;
                    case MessageType.ConfirmCancelDealResponse3D:
                    case MessageType.ExecutionReportResponse7X:
                    case MessageType.OrderChangeConfirmResponse7V:
                    case MessageType.OrderChangePutThroughFromBrokerResponse6W:
                        break; // TODO implement this
                    // should never ever receive type request
                    case MessageType.OrderCancelRequest7C:
                    case MessageType.NewOrderRequest7A:
                    case MessageType.OrderChangeRequest7M:
                    case MessageType.TradeReportRequest7G:
                    case MessageType.OrderChangePutThroughRequest7U:
                    case MessageType.OrderCancelPutThroughRequest7Q:
                        throw new ArgumentOutOfRangeException(
                            $"Invalid type, got request packet content type when expecting response packet content type: '{data.DataTransferPacketContent.MessageType}'"
                        );
                    case MessageType.Unknown:
                    default:
                        throw new ArgumentException("Invalid packet content type");
                }
                break;
            case PacketType.Logout:
                throw new SocketException(10054);
            case PacketType.Heartbeat:
            case PacketType.Logon:
            case PacketType.TestRequest:
            case PacketType.RecoveryRequest:
            case PacketType.RecoveryAcknowledge:
            case PacketType.RecoveryComplete:
            default:
                break;
        }
    }

    private Action<PublishContext<T>> SetContextMetaData<T>(Packet packet, string fisOrderId, string refOrderId) where T : class
    {
        return ctx =>
        {
            // var groupId = nameof(OnePortOrderEvent);
            var fisOrderIdTrimmed = IntegrationEventFactory.TrimPrefixWithoutEmpty(fisOrderId);

            if (fisOrderIdTrimmed != null)
            {
                var groupId = $"{nameof(OnePortOrderEvent)}_fis_order_id_{fisOrderIdTrimmed}";
                _logger.LogInformation("Group Id: {GroupId}", groupId);
                ctx.SetGroupId(groupId);
            }

            ctx.SetDeduplicationId($"{packet.Timestamp!.Value.DayOfYear}-{packet.Sequence.ToString()}");
        };
    }

    private void KeepAlive()
    {
        if (_keepAliveCancellationTokenSource != null) return;

        _keepAliveCancellationTokenSource = new CancellationTokenSource();
        Task.Run(_KeepAlive, _keepAliveCancellationTokenSource.Token);
    }

    private async Task _KeepAlive()
    {
        var lastLogged = DateTime.Now.ToShortTimeString().Remove(4);
        _logger.LogInformation("Starting keep-alive");

        try
        {
            while (!_keepAliveCancellationTokenSource?.Token.IsCancellationRequested ?? true)
            {
                var currentTime = DateTime.Now.ToShortTimeString().Remove(4);
                if (lastLogged != currentTime)
                {
                    _logger.LogDebug("keep-alive");
                    lastLogged = currentTime;
                }
                await Task.Delay(KeepAlivePeriod, _keepAliveCancellationTokenSource?.Token ?? CancellationToken.None);
                await _onePortClient.Send(new Packet(new PacketTest(KeepAliveMessage)));
            }
        }
        catch (IOException ioException)
        {
            if (!ioException.Message.ToLower().Contains("broken pipe"))
            {
                _logger.LogError(ioException, "Something went wrong");
                throw;
            }

            Reconnect();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, " Something went wrong");
            throw;
        }
        finally
        {
            _keepAliveCancellationTokenSource?.Dispose();
            _keepAliveCancellationTokenSource = null;
        }
    }

    private void Reconnect()
    {
        _logger.LogDebug("Start re-connecting to remote");
        _connectTask = null;
        var func = async () =>
        {
            await _onePortClient.Reconnect();
            _logger.LogDebug("Successfully re-connect to remote");
            await Logon();
        };

        _connectTask = func();
        _keepAliveCancellationTokenSource?.Dispose();
        _keepAliveCancellationTokenSource = null;
        KeepAlive();
    }

    private async Task Logon()
    {
        _logger.LogDebug("Logging in");
        var logon = new PacketLogon
        {
            LoginId = _username,
            Password = _password
        };
        var loginResponse = await _onePortClient.SendAndWaitResponse(new Packet(logon)
        {
            Sequence = _packetSequence + 1
        }, CancellationToken.None);

        if (loginResponse?.Data is not PacketHeartbeat { ResultFlag: true })
        {
            throw new Exception($"Unable to logon; {(loginResponse?.Data as PacketHeartbeat)?.Reserve}");
        }
        _logger.LogInformation("Successfully Logged In");
    }
}
