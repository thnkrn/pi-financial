using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Client.OnboardService.Model;
using Pi.Common.Features;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Domain.AccountOpening.Events.Models;
using Pi.Financial.FundService.Domain.Events;

namespace Pi.Financial.FundService.Application.Commands
{
    public record UploadFundAccountOpeningDocuments(Guid TicketId, Guid UserId, string CustomerCode, IEnumerable<Document> Documents, bool IsNdid, string? NdidRequestId, bool IsOpenSegregateAccount);

    public class UploadFundAccountOpeningDocumentsConsumer : IConsumer<UploadFundAccountOpeningDocuments>
    {
        private readonly ILogger<UploadFundAccountOpeningDocumentsConsumer> _logger;
        private readonly IFundConnextService _fundConnextService;
        private readonly ICustomerService _customerService;
        private readonly IFeatureService _featureService;
        private readonly IUserService _userService;
        private readonly IOnboardService _onboardService;

        public UploadFundAccountOpeningDocumentsConsumer(
            ILogger<UploadFundAccountOpeningDocumentsConsumer> logger,
            IFundConnextService fundConnextService,
            ICustomerService customerService,
            IFeatureService featureService,
            IUserService userService,
            IOnboardService onboardService)
        {
            _logger = logger;
            _fundConnextService = fundConnextService;
            _customerService = customerService;
            _featureService = featureService;
            _userService = userService;
            _onboardService = onboardService;
        }

        public async Task Consume(ConsumeContext<UploadFundAccountOpeningDocuments> context)
        {
            _logger.LogInformation("Consuming UploadFundAccountOpeningDocuments: {TicketId}",
                context.Message.TicketId);

            var customerInfo = await _customerService.GetCustomerInfo(context.Message.CustomerCode);
            var customerAccount = await _fundConnextService.GetCustomerAccountAsync(customerInfo.CardNumber, cancellationToken: context.CancellationToken);
            if (customerAccount == null)
            {
                throw new InvalidDataException($"Fund Customer account not found in FundConnext for card id: {customerInfo.CardNumber}");
            }

            var failedDocument = new List<string>();

            //Upload to Personal Document page
            await HandleSegregateAccount
            (
                context.Message.Documents,
                context.Message.UserId,
                context.Message.IsOpenSegregateAccount,
                context.Message.TicketId.ToString(),
                customerInfo.IdentificationCardType,
                customerInfo.CardNumber,
                failedDocument
            );

            //Upload to Account page
            await HandleUploadIndividualAccountDocuments
            (
                context.Message.Documents,
                context.Message.UserId,
                context.Message.TicketId.ToString(),
                customerInfo.IdentificationCardType,
                customerInfo.CardNumber,
                customerAccount.AccountId,
                failedDocument
            );

            if (failedDocument.Count != 0)
            {
                throw new UploadDocumentFailed(string.Join(", ", failedDocument));
            }

            await context.RespondAsync(new AccountOpeningDocumentsUploaded(context.Message.TicketId));
        }

        /// <summary>
        /// Upload consumer message's Documents to Account page and update documents that failed to upload in failedDocument.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="existedDocuments">An existed document from context message that will be uploaded to fund connext</param>
        /// <param name="isOpenSegregateAccount">'true' when you want upload documents to Account page</param>
        /// <param name="ticketId">ticketId</param>
        /// <param name="cardType">cardType</param>
        /// <param name="cardNumber">Citizen card number or passport number</param>
        /// <param name="failedDocument">This list will be directly modified and that the value will change for each document that can't be uploaded</param>
        public async Task HandleSegregateAccount
        (
            IEnumerable<Document> existedDocuments,
            Guid userId,
            bool isOpenSegregateAccount,
            string ticketId,
            IdentificationCardType cardType,
            string cardNumber,
            List<string> failedDocument
        )
        {
            var documentList = existedDocuments.ToList();

            List<Document> filteredDocumentList;
            if (_featureService.IsOn(Features.UploadSignatureIdCardToFundConnext))
            {
                await AddDocumentFromUserService(userId, documentList);
                filteredDocumentList = documentList
                    .Where(d => d.DocumentType
                        is DocumentType.FundOpenAccountForm
                        or DocumentType.SuitabilityForm
                        or DocumentType.Fatca
                        or DocumentType.IdCardForm
                        or DocumentType.Signature)
                    .ToList();
            }
            else
            {
                filteredDocumentList = documentList
                    .Where(d => d.DocumentType
                        is DocumentType.FundOpenAccountForm
                        or DocumentType.SuitabilityForm
                        or DocumentType.Fatca
                        or DocumentType.IdCardForm)
                    .ToList();
            }

            if (_featureService.IsOn(Features.UploadFileSegregate) || isOpenSegregateAccount)
            {
                foreach (var document in filteredDocumentList)
                {
                    try
                    {
                        await _fundConnextService.UploadIndividualCustomerDocuments(
                            ticketId,
                            cardType,
                            cardNumber,
                            document
                        );
                    }
                    catch (Exception e)
                    {
                        failedDocument.Add(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Upload consumer message's Documents to Personal Document page and update documents that failed to upload in failedDocument.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="existedDocuments">An existed document from context message that will be uploaded to Fund Connext</param>
        /// <param name="ticketId">ticketId</param>
        /// <param name="cardType">cardType</param>
        /// <param name="cardNumber">Citizen card number or Passport number</param>
        /// <param name="accountId">accountId</param>
        /// <param name="failedDocument">This list will be directly modified and that the value will change for each document that can't be uploaded</param>
        public async Task HandleUploadIndividualAccountDocuments
        (
            IEnumerable<Document> existedDocuments,
            Guid userId,
            string ticketId,
            IdentificationCardType cardType,
            string cardNumber,
            string accountId,
            List<string> failedDocument
        )
        {
            var documentList = existedDocuments.ToList();

            if (_featureService.IsOn(Features.UploadSignatureIdCardToFundConnext))
            {
                await AddDocumentFromUserService(userId, documentList);
            }

            documentList = documentList.Select(d => d with
            {
                DocumentType = d.DocumentType
                    is DocumentType.FundOpenAccountForm
                    or DocumentType.IdCardForm
                    or DocumentType.SuitabilityForm
                    or DocumentType.Fatca
                    or DocumentType.Signature
                    ? DocumentType.AccountInformation
                    : d.DocumentType,
                OriginalDocumentType = d.DocumentType
            }).ToList();

            var filteredDocumentList = documentList
                .Where(d => d.DocumentType
                    is DocumentType.AccountInformation
                    or DocumentType.BankAccount
                    or DocumentType.Amendment)
                .ToList();

            foreach (var document in filteredDocumentList)
            {
                try
                {
                    await _fundConnextService.UploadIndividualAccountDocuments(
                        ticketId,
                        cardType,
                        cardNumber,
                        accountId,
                        document);
                }
                catch (Exception e)
                {
                    failedDocument.Add(e.Message);
                }
            }
        }

        /// <summary>
        /// For all <c>userId</c>'s uploaded documents, add documents of certain types to the <paramref name="existedDocuments"/> parameter based on the provided options.
        /// Supported documents are: Signature, CitizenCard, BookBank, and CitizenCardPortrait.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="existedDocuments">A list of existed documents or empty list that you want to modify</param>
        /// <param name="addSignature">Choose to add Signature document type. Default = true.</param>
        /// <param name="addIdCard">Choose to add CitizenCard document type. Default = true.</param>
        /// <param name="addBookBank">Choose to add BookBank document type. Default = false.</param>
        /// <param name="addPortrait">Choose to add CitizenCardPortrait document type. Default = false.</param>
        public async Task AddDocumentFromUserService
        (
            Guid userId,
            ICollection<Document> existedDocuments,
            bool addSignature = true,
            bool addIdCard = true,
            bool addBookBank = false,
            bool addPortrait = false
        )
        {
            var userDocuments = await GetOnboardUserDocumentsByUserId(userId);

            int signatureDocCount = 0;
            int idCardDocCount = 0;
            int bookBankCount = 0;
            int portraitCount = 0;
            foreach (PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto userDocument in userDocuments)
            {
                switch (userDocument.DocumentType)
                {
                    case PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.Signature
                        when signatureDocCount < 1 && addSignature:
                        {
                            Document signature = new(DocumentType.Signature, userDocument.FileUrl);
                            existedDocuments.Add(signature);
                            signatureDocCount++;
                            break;
                        }
                    case PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.CitizenCard
                        when idCardDocCount < 1 && addIdCard:
                        {
                            Document idCardPhoto = new(DocumentType.IdCardForm, userDocument.FileUrl);
                            existedDocuments.Add(idCardPhoto);
                            idCardDocCount++;
                            break;
                        }
                    case PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.BookBank
                        when bookBankCount < 1 && addBookBank:
                        {
                            Document bookBank = new(DocumentType.BankAccount, userDocument.FileUrl);
                            existedDocuments.Add(bookBank);
                            bookBankCount++;
                            break;
                        }
                    case PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.CitizenCardPortrait
                        when portraitCount < 1 && addPortrait:
                        {
                            Document portrait = new(DocumentType.IdCardForm, userDocument.FileUrl);
                            existedDocuments.Add(portrait);
                            portraitCount++;
                            break;
                        }
                    case PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.Statement:
                    case PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.Selfie:
                    case PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.FatcaCrs:
                    case null:
                        break;
                    default:
                        _logger.LogError("AddDocumentFromUserService() in UploadFundAccountOpeningDocumentsConsumer out of range");
                        break;
                }
            }

            if (signatureDocCount == 0 && addSignature) _logger.LogInformation("Signature document not found");
            if (idCardDocCount == 0 && addIdCard) _logger.LogInformation("IdCard document not found");
            if (bookBankCount == 0 && addBookBank) _logger.LogInformation("BookBank document not found");
            if (portraitCount == 0 && addPortrait) _logger.LogInformation("Portrait document not found");

        }

        private async Task<List<PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto>> GetOnboardUserDocumentsByUserId(
            Guid userId, CancellationToken cancellationToken = default)
        {
            if (_featureService.IsOn(Features.UseOnboardUserDocument))
            {
                return await _onboardService.GetDocumentByUserId(userId, cancellationToken);
            }

            var userDocuments = await _userService.GetDocumentByUserId(userId);
            List<PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto> onboardUserDocuments = [];
            foreach (var userDocument in userDocuments)
            {
                if (!Enum.TryParse<PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum>(userDocument.DocumentType.ToString(), out var documentType))
                {
                    throw new InvalidCastException($"Unable to parse document type {userDocument.DocumentType} to onboard user document dto format");
                }

                onboardUserDocuments.Add(new PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto(
                    userDocument.FileUrl, userDocument.FileName, documentType, userDocument.CreatedAt));
            }

            return onboardUserDocuments;
        }
    }
}
