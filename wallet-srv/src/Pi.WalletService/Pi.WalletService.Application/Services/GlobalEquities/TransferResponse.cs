namespace Pi.WalletService.Application.Services.GlobalEquities;

public record TransferResponse(string SourceAccountId, string TargetAccountId, string Asset, decimal Amount, string SequenceId);
