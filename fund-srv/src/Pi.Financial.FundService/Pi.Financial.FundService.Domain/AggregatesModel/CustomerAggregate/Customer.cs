namespace Pi.Financial.FundService.Domain.AggregatesModel.CustomerAggregate;

public class Customer
{
    public DateOnly TransDate { get; set; }

    public decimal RequestTime { get; set; }

    public char FlagProcess { get; set; }

    public decimal AppId { get; set; }

    public string Status { get; set; } = null!;

    public string NdidStatus { get; set; } = null!;

    public string Types { get; set; } = null!;

    public string VerifiType { get; set; } = null!;

    public string ContractNo { get; set; } = null!;

    public string DdrBankCode { get; set; } = null!;

    public string DdrBankAccountNo { get; set; } = null!;

    public string DdrStatus { get; set; } = null!;

    public string DdrUrl { get; set; } = null!;

    public string CreatedTime { get; set; } = null!;

    public string LastUpdatedTime { get; set; } = null!;

    public string SubmittedTime { get; set; } = null!;

    public decimal UUserid { get; set; }

    public string UCid { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string IdCardType { get; set; } = null!;

    public string CardIssueDate { get; set; } = null!;

    public string CardExpiryDate { get; set; } = null!;

    public string TTitle { get; set; } = null!;

    public string TFname { get; set; } = null!;

    public string TLname { get; set; } = null!;

    public string ETitle { get; set; } = null!;

    public string EFname { get; set; } = null!;

    public string ELname { get; set; } = null!;

    public string Tel { get; set; } = null!;

    public string Fax { get; set; } = null!;

    public string OfficeTel { get; set; } = null!;

    public string OfficeTelExt { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string BirthDate { get; set; } = null!;

    public string Nationality { get; set; } = null!;

    public string Taxid { get; set; } = null!;

    public string MailSameFlag { get; set; } = null!;

    public string MailNo { get; set; } = null!;

    public string MailMoo { get; set; } = null!;

    public string MailVillage { get; set; } = null!;

    public string MailBuilding { get; set; } = null!;

    public string MailFloor { get; set; } = null!;

    public string MailSoi { get; set; } = null!;

    public string MailRoad { get; set; } = null!;

    public string MailSubDistrict { get; set; } = null!;

    public string MailDistrict { get; set; } = null!;

    public string MailProvince { get; set; } = null!;

    public string MailCountry { get; set; } = null!;

    public string MailPostal { get; set; } = null!;

    public string ContSameFlag { get; set; } = null!;

    public string ContNo { get; set; } = null!;

    public string ContMoo { get; set; } = null!;

    public string ContVillage { get; set; } = null!;

    public string ContBuilding { get; set; } = null!;

    public string ContFloor { get; set; } = null!;

    public string ContSoi { get; set; } = null!;

    public string ContRoad { get; set; } = null!;

    public string ContSubDistrict { get; set; } = null!;

    public string ContDistrict { get; set; } = null!;

    public string ContProvince { get; set; } = null!;

    public string ContCountry { get; set; } = null!;

    public string ContPostal { get; set; } = null!;

    public string ResiNo { get; set; } = null!;

    public string ResiMoo { get; set; } = null!;

    public string ResiVillage { get; set; } = null!;

    public string ResiBuilding { get; set; } = null!;

    public string ResiFloor { get; set; } = null!;

    public string ResiSoi { get; set; } = null!;

    public string ResiRoad { get; set; } = null!;

    public string ResiSubDistrict { get; set; } = null!;

    public string ResiDistrict { get; set; } = null!;

    public string ResiProvince { get; set; } = null!;

    public string ResiCountry { get; set; } = null!;

    public string ResiPostal { get; set; } = null!;

    public string WorkNo { get; set; } = null!;

    public string WorkMoo { get; set; } = null!;

    public string WorkVillage { get; set; } = null!;

    public string WorkBuilding { get; set; } = null!;

    public string WorkFloor { get; set; } = null!;

    public string WorkSoi { get; set; } = null!;

    public string WorkRoad { get; set; } = null!;

    public string WorkSubDistrict { get; set; } = null!;

    public string WorkDistrict { get; set; } = null!;

    public string WorkProvince { get; set; } = null!;

    public string WorkCountry { get; set; } = null!;

    public string WorkPostal { get; set; } = null!;

    public char CensusFlag { get; set; }

    public string MailingMethod { get; set; } = null!;

    public string RedempBankCode { get; set; } = null!;

    public string RedempBankBranchCode { get; set; } = null!;

    public string RedempBankAccountNo { get; set; } = null!;

    public string RedempBankAccountName { get; set; } = null!;

    public string RedempBankDefault { get; set; } = null!;

    public char AtsService { get; set; }

    public char EdvdService { get; set; }

    public string EdvdBankCode { get; set; } = null!;

    public string EdvdBankBranchCode { get; set; } = null!;

    public string EdvdBankAccountNo { get; set; } = null!;

    public string FamilyMarital { get; set; } = null!;

    public string FamilyTitle { get; set; } = null!;

    public string FamilyFname { get; set; } = null!;

    public string FamilyLname { get; set; } = null!;

    public string FamilyEfname { get; set; } = null!;

    public string FamilyElname { get; set; } = null!;

    public string FamilyPhone { get; set; } = null!;

    public string FamilyIdCardType { get; set; } = null!;

    public string FamilyCardNo { get; set; } = null!;

    public string FamilyCardExpiryDate { get; set; } = null!;

    public string ChildAmt { get; set; } = null!;

    public string ChildIdCardType1 { get; set; } = null!;

    public string ChildCardNo1 { get; set; } = null!;

    public string ChildCardExpiryDate1 { get; set; } = null!;

    public string ChildCountry1 { get; set; } = null!;

    public string ChildTitle1 { get; set; } = null!;

    public string ChildFname1 { get; set; } = null!;

    public string ChildLname1 { get; set; } = null!;

    public string ChildBirthday1 { get; set; } = null!;

    public string ChildIdCardType2 { get; set; } = null!;

    public string ChildCardNo2 { get; set; } = null!;

    public string ChildCardExpiryDate2 { get; set; } = null!;

    public string ChildCountry2 { get; set; } = null!;

    public string ChildTitle2 { get; set; } = null!;

    public string ChildFname2 { get; set; } = null!;

    public string ChildLname2 { get; set; } = null!;

    public string ChildBirthday2 { get; set; } = null!;

    public string ChildIdCardType3 { get; set; } = null!;

    public string ChildCardNo3 { get; set; } = null!;

    public string ChildCardExpiryDate3 { get; set; } = null!;

    public string ChildCountry3 { get; set; } = null!;

    public string ChildTitle3 { get; set; } = null!;

    public string ChildFname3 { get; set; } = null!;

    public string ChildLname3 { get; set; } = null!;

    public string ChildBirthday3 { get; set; } = null!;

    public string EmerName { get; set; } = null!;

    public string EmerSurname { get; set; } = null!;

    public string EmerRelate { get; set; } = null!;

    public string EmerPhone { get; set; } = null!;

    public string BeneficRelationCode { get; set; } = null!;

    public string BeneficRelation { get; set; } = null!;

    public string BeneficName { get; set; } = null!;

    public string BeneficSurname { get; set; } = null!;

    public string BeneficBirthDate { get; set; } = null!;

    public string BeneficIdCardType { get; set; } = null!;

    public string BeneficCardNo { get; set; } = null!;

    public string BeneficCardExpiryDate { get; set; } = null!;

    public string BeneficCardExpiry { get; set; } = null!;

    public string BeneficNo { get; set; } = null!;

    public string BeneficMoo { get; set; } = null!;

    public string BeneficVillage { get; set; } = null!;

    public string BeneficBuilding { get; set; } = null!;

    public string BeneficFloor { get; set; } = null!;

    public string BeneficSoi { get; set; } = null!;

    public string BeneficRoad { get; set; } = null!;

    public string BeneficSubDistrict { get; set; } = null!;

    public string BeneficDistrict { get; set; } = null!;

    public string BeneficProvince { get; set; } = null!;

    public string BeneficCountry { get; set; } = null!;

    public string BeneficPostal { get; set; } = null!;

    public string TraderRefer { get; set; } = null!;

    public string TraderId { get; set; } = null!;

    public string Ibacode { get; set; } = null!;

    public string Vipflag { get; set; } = null!;

    public string VipquestionCode { get; set; } = null!;

    public string Vipquestion { get; set; } = null!;

    public string FinanOccupationCode { get; set; } = null!;

    public string FinanOccupation { get; set; } = null!;

    public string FinanBusinessCode { get; set; } = null!;

    public string FinanBusiness { get; set; } = null!;

    public string FinanCompany { get; set; } = null!;

    public string FinanPosition { get; set; } = null!;

    public string FinanMonthlyIncomeCode { get; set; } = null!;

    public string FinanMonthlyIncome { get; set; } = null!;

    public string FinanIncSource1 { get; set; } = null!;

    public string FinanIncSource2 { get; set; } = null!;

    public string FinanIncSource3 { get; set; } = null!;

    public string FinanIncSource4 { get; set; } = null!;

    public string FinanIncSource5 { get; set; } = null!;

    public string FinanIncSource6 { get; set; } = null!;

    public string FinanIncSource7 { get; set; } = null!;

    public string FinanIncSourceCountry { get; set; } = null!;

    public string FinanAssetValue { get; set; } = null!;

    public string InvestId1 { get; set; } = null!;

    public string InvestLabel1 { get; set; } = null!;

    public string InvestId2 { get; set; } = null!;

    public string InvestLabel2 { get; set; } = null!;

    public string InvestId3 { get; set; } = null!;

    public string InvestLabel3 { get; set; } = null!;

    public string InvestId4 { get; set; } = null!;

    public string InvestLabel4 { get; set; } = null!;

    public string InvestId5 { get; set; } = null!;

    public string InvestLabel5 { get; set; } = null!;

    public string InvestId6 { get; set; } = null!;

    public string InvestLabel6 { get; set; } = null!;

    public string InvestId7 { get; set; } = null!;

    public string InvestLabel7 { get; set; } = null!;

    public string InvestId8 { get; set; } = null!;

    public string InvestLabel8 { get; set; } = null!;

    public string PoliticalPerson { get; set; } = null!;

    public string PoliticalPosition { get; set; } = null!;

    public string FatcaDate { get; set; } = null!;

    public string FatcaQId1 { get; set; } = null!;

    public string FatcaCId1 { get; set; } = null!;

    public string FatcaQId2 { get; set; } = null!;

    public string FatcaCId2 { get; set; } = null!;

    public string FatcaQId3 { get; set; } = null!;

    public string FatcaCId3 { get; set; } = null!;

    public string FatcaQId4 { get; set; } = null!;

    public string FatcaCId4 { get; set; } = null!;

    public string FatcaQId5 { get; set; } = null!;

    public string FatcaCId5 { get; set; } = null!;

    public string FatcaQId6 { get; set; } = null!;

    public string FatcaCId6 { get; set; } = null!;

    public string FatcaQId7 { get; set; } = null!;

    public string FatcaCId7 { get; set; } = null!;

    public string FatcaQId8 { get; set; } = null!;

    public string FatcaCId8 { get; set; } = null!;

    public string SuitQId1 { get; set; } = null!;

    public string SuitQLebel1 { get; set; } = null!;

    public string SuitCId1 { get; set; } = null!;

    public string SuitCLabel1 { get; set; } = null!;

    public decimal SuitCScore1 { get; set; }

    public string SuitQId2 { get; set; } = null!;

    public string SuitQLebel2 { get; set; } = null!;

    public string SuitCId2 { get; set; } = null!;

    public string SuitCLabel2 { get; set; } = null!;

    public decimal SuitCScore2 { get; set; }

    public string SuitQId3 { get; set; } = null!;

    public string SuitQLebel3 { get; set; } = null!;

    public string SuitCId3 { get; set; } = null!;

    public string SuitCLabel3 { get; set; } = null!;

    public decimal SuitCScore3 { get; set; }

    public string SuitQId4 { get; set; } = null!;

    public string SuitQLebel4 { get; set; } = null!;

    public string SuitCId41 { get; set; } = null!;

    public string SuitCLabel41 { get; set; } = null!;

    public decimal SuitCScore41 { get; set; }

    public string SuitCId42 { get; set; } = null!;

    public string SuitCLabel42 { get; set; } = null!;

    public decimal SuitCScore42 { get; set; }

    public string SuitCId43 { get; set; } = null!;

    public string SuitCLabel43 { get; set; } = null!;

    public decimal SuitCScore43 { get; set; }

    public string SuitCId44 { get; set; } = null!;

    public string SuitCLabel44 { get; set; } = null!;

    public decimal SuitCScore44 { get; set; }

    public string SuitQId5 { get; set; } = null!;

    public string SuitQLebel5 { get; set; } = null!;

    public string SuitCId5 { get; set; } = null!;

    public string SuitCLabel5 { get; set; } = null!;

    public decimal SuitCScore5 { get; set; }

    public string SuitQId6 { get; set; } = null!;

    public string SuitQLebel6 { get; set; } = null!;

    public string SuitCId6 { get; set; } = null!;

    public string SuitCLabel6 { get; set; } = null!;

    public decimal SuitCScore6 { get; set; }

    public string SuitQId7 { get; set; } = null!;

    public string SuitQLebel7 { get; set; } = null!;

    public string SuitCId7 { get; set; } = null!;

    public string SuitCLabel7 { get; set; } = null!;

    public decimal SuitCScore7 { get; set; }

    public string SuitQId8 { get; set; } = null!;

    public string SuitQLebel8 { get; set; } = null!;

    public string SuitCId8 { get; set; } = null!;

    public string SuitCLabel8 { get; set; } = null!;

    public decimal SuitCScore8 { get; set; }

    public string SuitQId9 { get; set; } = null!;

    public string SuitQLebel9 { get; set; } = null!;

    public string SuitCId9 { get; set; } = null!;

    public string SuitCLabel9 { get; set; } = null!;

    public decimal SuitCScore9 { get; set; }

    public string SuitQId10 { get; set; } = null!;

    public string SuitQLebel10 { get; set; } = null!;

    public string SuitCId10 { get; set; } = null!;

    public string SuitCLabel10 { get; set; } = null!;

    public decimal SuitCScore10 { get; set; }

    public string SuitQId11 { get; set; } = null!;

    public string SuitQLebel11 { get; set; } = null!;

    public string SuitCId11 { get; set; } = null!;

    public string SuitCLabel11 { get; set; } = null!;

    public decimal SuitCScore11 { get; set; }

    public string SuitQId12 { get; set; } = null!;

    public string SuitQLebel12 { get; set; } = null!;

    public string SuitCId12 { get; set; } = null!;

    public string SuitCLabel12 { get; set; } = null!;

    public decimal SuitCScore12 { get; set; }

    public string SuitQId13 { get; set; } = null!;

    public string SuitQLebel13 { get; set; } = null!;

    public string SuitCId13 { get; set; } = null!;

    public string SuitCLabel13 { get; set; } = null!;

    public decimal SuitCScore13 { get; set; }

    public string SuitQId14 { get; set; } = null!;

    public string SuitQLebel14 { get; set; } = null!;

    public string SuitCId14 { get; set; } = null!;

    public string SuitCLabel14 { get; set; } = null!;

    public decimal SuitCScore14 { get; set; }

    public decimal SuitSum { get; set; }

    public string SuitRiskLevel { get; set; } = null!;

    public char ZipFile { get; set; }

    public string ZipPassword { get; set; } = null!;

    public char FlagConv { get; set; }

    public char FlagReject { get; set; }

    public string FlagRejectNotes { get; set; } = null!;

    public string FlagRejectUser { get; set; } = null!;

    public DateTime? FlagRejectDatetime { get; set; }

    public char SttUpdateStatus { get; set; }

    public string SttUpdateStatusUser { get; set; } = null!;

    public DateTime? SttUpdateStatusDatetime { get; set; }

    public string SttUpdateStatusCode { get; set; } = null!;

    public string Notes { get; set; } = null!;

    public short IsActive { get; set; }

    public string EntryUser { get; set; } = null!;

    public DateTime? EntryDatetime { get; set; }

    public string LastEntryUser { get; set; } = null!;

    public DateTime? LastEntryDatetime { get; set; }

    public string PdpaCsQId1 { get; set; } = null!;

    public string PdpaCsAId1 { get; set; } = null!;

    public string PdpaCsALabel1 { get; set; } = null!;

    public string PdpaCsQId2 { get; set; } = null!;

    public string PdpaCsAId2 { get; set; } = null!;

    public string PdpaCsALabel2 { get; set; } = null!;

    public string PdpaCsQId3 { get; set; } = null!;

    public string PdpaCsAId3 { get; set; } = null!;

    public string PdpaCsALabel3 { get; set; } = null!;

    public char FlagPdpaExport { get; set; }

    public string FlagPdpaExportUser { get; set; } = null!;

    public DateTime? FlagPdpaExportDatetime { get; set; }

    public string? KnowledgeQuestion1 { get; set; }

    public string? KnowledgeQuestion2 { get; set; }

    public string? KnowledgeQuestion3 { get; set; }

    public string? Spousecardnumber { get; set; }

    public string? NdidReferenceId { get; set; }
}
