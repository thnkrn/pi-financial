using Microsoft.EntityFrameworkCore;
using Pi.Financial.FundService.Domain.AggregatesModel.CustomerAggregate;

namespace Pi.Financial.FundService.Infrastructure;

public partial class CustomerDbContext : DbContext
{
    public CustomerDbContext()
    {
    }

    public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> EopenStts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("eopen_stt");

            entity.HasIndex(e => new { e.TransDate, e.RequestTime, e.Types }, "idx_eopenstt01");

            entity.HasIndex(e => new { e.TransDate, e.RequestTime, e.AppId }, "idx_eopenstt02");

            entity.HasIndex(e => new { e.AppId, e.ContractNo, e.SubmittedTime, e.LastUpdatedTime }, "idx_eopenstt03");

            entity.Property(e => e.AppId)
                .HasPrecision(16)
                .HasColumnName("app_id");
            entity.Property(e => e.AtsService)
                .HasMaxLength(1)
                .HasDefaultValueSql("''::bpchar")
                .HasColumnName("ats_service");
            entity.Property(e => e.BeneficBirthDate)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_birth_date");
            entity.Property(e => e.BeneficBuilding)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_building");
            entity.Property(e => e.BeneficCardExpiry)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_card_expiry");
            entity.Property(e => e.BeneficCardExpiryDate)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_card_expiry_date");
            entity.Property(e => e.BeneficCardNo)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_card_no");
            entity.Property(e => e.BeneficCountry)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_country");
            entity.Property(e => e.BeneficDistrict)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_district");
            entity.Property(e => e.BeneficFloor)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_floor");
            entity.Property(e => e.BeneficIdCardType)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_id_card_type");
            entity.Property(e => e.BeneficMoo)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_moo");
            entity.Property(e => e.BeneficName)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_name");
            entity.Property(e => e.BeneficNo)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_no");
            entity.Property(e => e.BeneficPostal)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_postal");
            entity.Property(e => e.BeneficProvince)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_province");
            entity.Property(e => e.BeneficRelation)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_relation");
            entity.Property(e => e.BeneficRelationCode)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_relation_code");
            entity.Property(e => e.BeneficRoad)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_road");
            entity.Property(e => e.BeneficSoi)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_soi");
            entity.Property(e => e.BeneficSubDistrict)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_sub_district");
            entity.Property(e => e.BeneficSurname)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_surname");
            entity.Property(e => e.BeneficVillage)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("benefic_village");
            entity.Property(e => e.BirthDate)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("birth_date");
            entity.Property(e => e.CardExpiryDate)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("card_expiry_date");
            entity.Property(e => e.CardIssueDate)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("card_issue_date");
            entity.Property(e => e.CensusFlag)
                .HasMaxLength(1)
                .HasDefaultValueSql("''::bpchar")
                .HasColumnName("census_flag");
            entity.Property(e => e.ChildAmt)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_amt");
            entity.Property(e => e.ChildBirthday1)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_birthday1");
            entity.Property(e => e.ChildBirthday2)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_birthday2");
            entity.Property(e => e.ChildBirthday3)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_birthday3");
            entity.Property(e => e.ChildCardExpiryDate1)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_card_expiry_date1");
            entity.Property(e => e.ChildCardExpiryDate2)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_card_expiry_date2");
            entity.Property(e => e.ChildCardExpiryDate3)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_card_expiry_date3");
            entity.Property(e => e.ChildCardNo1)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_card_no1");
            entity.Property(e => e.ChildCardNo2)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_card_no2");
            entity.Property(e => e.ChildCardNo3)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_card_no3");
            entity.Property(e => e.ChildCountry1)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_country1");
            entity.Property(e => e.ChildCountry2)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_country2");
            entity.Property(e => e.ChildCountry3)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_country3");
            entity.Property(e => e.ChildFname1)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_fname1");
            entity.Property(e => e.ChildFname2)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_fname2");
            entity.Property(e => e.ChildFname3)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_fname3");
            entity.Property(e => e.ChildIdCardType1)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_id_card_type1");
            entity.Property(e => e.ChildIdCardType2)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_id_card_type2");
            entity.Property(e => e.ChildIdCardType3)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_id_card_type3");
            entity.Property(e => e.ChildLname1)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_lname1");
            entity.Property(e => e.ChildLname2)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_lname2");
            entity.Property(e => e.ChildLname3)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_lname3");
            entity.Property(e => e.ChildTitle1)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_title1");
            entity.Property(e => e.ChildTitle2)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_title2");
            entity.Property(e => e.ChildTitle3)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("child_title3");
            entity.Property(e => e.ContBuilding)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("cont_building");
            entity.Property(e => e.ContCountry)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("cont_country");
            entity.Property(e => e.ContDistrict)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("cont_district");
            entity.Property(e => e.ContFloor)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("cont_floor");
            entity.Property(e => e.ContMoo)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("cont_moo");
            entity.Property(e => e.ContNo)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("cont_no");
            entity.Property(e => e.ContPostal)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("cont_postal");
            entity.Property(e => e.ContProvince)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("cont_province");
            entity.Property(e => e.ContRoad)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("cont_road");
            entity.Property(e => e.ContSameFlag)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("cont_same_flag");
            entity.Property(e => e.ContSoi)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("cont_soi");
            entity.Property(e => e.ContSubDistrict)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("cont_sub_district");
            entity.Property(e => e.ContVillage)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("cont_village");
            entity.Property(e => e.ContractNo)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("contract_no");
            entity.Property(e => e.CreatedTime)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("created_time");
            entity.Property(e => e.DdrBankAccountNo)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("ddr_bank_account_no");
            entity.Property(e => e.DdrBankCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("ddr_bank_code");
            entity.Property(e => e.DdrStatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("ddr_status");
            entity.Property(e => e.DdrUrl)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("ddr_url");
            entity.Property(e => e.EFname)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("e_fname");
            entity.Property(e => e.ELname)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("e_lname");
            entity.Property(e => e.ETitle)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("e_title");
            entity.Property(e => e.EdvdBankAccountNo)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("edvd_bank_account_no");
            entity.Property(e => e.EdvdBankBranchCode)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("edvd_bank_branch_code");
            entity.Property(e => e.EdvdBankCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("edvd_bank_code");
            entity.Property(e => e.EdvdService)
                .HasMaxLength(1)
                .HasDefaultValueSql("''::bpchar")
                .HasColumnName("edvd_service");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("email");
            entity.Property(e => e.EmerName)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("emer_name");
            entity.Property(e => e.EmerPhone)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("emer_phone");
            entity.Property(e => e.EmerRelate)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("emer_relate");
            entity.Property(e => e.EmerSurname)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("emer_surname");
            entity.Property(e => e.EntryDatetime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("entry_datetime");
            entity.Property(e => e.EntryUser)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("entry_user");
            entity.Property(e => e.FamilyCardExpiryDate)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("family_card_expiry_date");
            entity.Property(e => e.FamilyCardNo)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("family_card_no");
            entity.Property(e => e.FamilyEfname)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("family_efname");
            entity.Property(e => e.FamilyElname)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("family_elname");
            entity.Property(e => e.FamilyFname)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("family_fname");
            entity.Property(e => e.FamilyIdCardType)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("family_id_card_type");
            entity.Property(e => e.FamilyLname)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("family_lname");
            entity.Property(e => e.FamilyMarital)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("family_marital");
            entity.Property(e => e.FamilyPhone)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("family_phone");
            entity.Property(e => e.FamilyTitle)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("family_title");
            entity.Property(e => e.FatcaCId1)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_c_id1");
            entity.Property(e => e.FatcaCId2)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_c_id2");
            entity.Property(e => e.FatcaCId3)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_c_id3");
            entity.Property(e => e.FatcaCId4)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_c_id4");
            entity.Property(e => e.FatcaCId5)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_c_id5");
            entity.Property(e => e.FatcaCId6)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_c_id6");
            entity.Property(e => e.FatcaCId7)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_c_id7");
            entity.Property(e => e.FatcaCId8)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_c_id8");
            entity.Property(e => e.FatcaDate)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_date");
            entity.Property(e => e.FatcaQId1)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_q_id1");
            entity.Property(e => e.FatcaQId2)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_q_id2");
            entity.Property(e => e.FatcaQId3)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_q_id3");
            entity.Property(e => e.FatcaQId4)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_q_id4");
            entity.Property(e => e.FatcaQId5)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_q_id5");
            entity.Property(e => e.FatcaQId6)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_q_id6");
            entity.Property(e => e.FatcaQId7)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_q_id7");
            entity.Property(e => e.FatcaQId8)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fatca_q_id8");
            entity.Property(e => e.Fax)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("fax");
            entity.Property(e => e.FinanAssetValue)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_asset_value");
            entity.Property(e => e.FinanBusiness)
                .HasMaxLength(200)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_business");
            entity.Property(e => e.FinanBusinessCode)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_business_code");
            entity.Property(e => e.FinanCompany)
                .HasMaxLength(200)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_company");
            entity.Property(e => e.FinanIncSource1)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_inc_source1");
            entity.Property(e => e.FinanIncSource2)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_inc_source2");
            entity.Property(e => e.FinanIncSource3)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_inc_source3");
            entity.Property(e => e.FinanIncSource4)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_inc_source4");
            entity.Property(e => e.FinanIncSource5)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_inc_source5");
            entity.Property(e => e.FinanIncSource6)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_inc_source6");
            entity.Property(e => e.FinanIncSource7)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_inc_source7");
            entity.Property(e => e.FinanIncSourceCountry)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_inc_source_country");
            entity.Property(e => e.FinanMonthlyIncome)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_monthly_income");
            entity.Property(e => e.FinanMonthlyIncomeCode)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_monthly_income_code");
            entity.Property(e => e.FinanOccupation)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_occupation");
            entity.Property(e => e.FinanOccupationCode)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_occupation_code");
            entity.Property(e => e.FinanPosition)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("finan_position");
            entity.Property(e => e.FlagConv)
                .HasMaxLength(1)
                .HasDefaultValueSql("'N'::bpchar")
                .HasColumnName("flag_conv");
            entity.Property(e => e.FlagPdpaExport)
                .HasMaxLength(1)
                .HasDefaultValueSql("'N'::bpchar")
                .HasColumnName("flag_pdpa_export");
            entity.Property(e => e.FlagPdpaExportDatetime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("flag_pdpa_export_datetime");
            entity.Property(e => e.FlagPdpaExportUser)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("flag_pdpa_export_user");
            entity.Property(e => e.FlagProcess)
                .HasMaxLength(1)
                .HasDefaultValueSql("''::bpchar")
                .HasColumnName("flag_process");
            entity.Property(e => e.FlagReject)
                .HasMaxLength(1)
                .HasDefaultValueSql("'N'::bpchar")
                .HasColumnName("flag_reject");
            entity.Property(e => e.FlagRejectDatetime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("flag_reject_datetime");
            entity.Property(e => e.FlagRejectNotes)
                .HasDefaultValueSql("''::text")
                .HasColumnName("flag_reject_notes");
            entity.Property(e => e.FlagRejectUser)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("flag_reject_user");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("gender");
            entity.Property(e => e.Ibacode)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("ibacode");
            entity.Property(e => e.IdCardType)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("id_card_type");
            entity.Property(e => e.InvestId1)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_id1");
            entity.Property(e => e.InvestId2)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_id2");
            entity.Property(e => e.InvestId3)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_id3");
            entity.Property(e => e.InvestId4)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_id4");
            entity.Property(e => e.InvestId5)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_id5");
            entity.Property(e => e.InvestId6)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_id6");
            entity.Property(e => e.InvestId7)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_id7");
            entity.Property(e => e.InvestId8)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_id8");
            entity.Property(e => e.InvestLabel1)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_label1");
            entity.Property(e => e.InvestLabel2)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_label2");
            entity.Property(e => e.InvestLabel3)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_label3");
            entity.Property(e => e.InvestLabel4)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_label4");
            entity.Property(e => e.InvestLabel5)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_label5");
            entity.Property(e => e.InvestLabel6)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_label6");
            entity.Property(e => e.InvestLabel7)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_label7");
            entity.Property(e => e.InvestLabel8)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("invest_label8");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("1")
                .HasColumnName("is_active");
            entity.Property(e => e.KnowledgeQuestion1).HasColumnName("knowledge_question_1");
            entity.Property(e => e.KnowledgeQuestion2).HasColumnName("knowledge_question_2");
            entity.Property(e => e.KnowledgeQuestion3).HasColumnName("knowledge_question_3");
            entity.Property(e => e.LastEntryDatetime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_entry_datetime");
            entity.Property(e => e.LastEntryUser)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("last_entry_user");
            entity.Property(e => e.LastUpdatedTime)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("last_updated_time");
            entity.Property(e => e.MailBuilding)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mail_building");
            entity.Property(e => e.MailCountry)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mail_country");
            entity.Property(e => e.MailDistrict)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mail_district");
            entity.Property(e => e.MailFloor)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mail_floor");
            entity.Property(e => e.MailMoo)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mail_moo");
            entity.Property(e => e.MailNo)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mail_no");
            entity.Property(e => e.MailPostal)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mail_postal");
            entity.Property(e => e.MailProvince)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mail_province");
            entity.Property(e => e.MailRoad)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mail_road");
            entity.Property(e => e.MailSameFlag)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mail_same_flag");
            entity.Property(e => e.MailSoi)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mail_soi");
            entity.Property(e => e.MailSubDistrict)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mail_sub_district");
            entity.Property(e => e.MailVillage)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mail_village");
            entity.Property(e => e.MailingMethod)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mailing_method");
            entity.Property(e => e.Mobile)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("mobile");
            entity.Property(e => e.Nationality)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("nationality");
            entity.Property(e => e.NdidReferenceId).HasColumnName("ndid_reference_id");
            entity.Property(e => e.NdidStatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("ndid_status");
            entity.Property(e => e.Notes)
                .HasDefaultValueSql("''::text")
                .HasColumnName("notes");
            entity.Property(e => e.OfficeTel)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("office_tel");
            entity.Property(e => e.OfficeTelExt)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("office_tel_ext");
            entity.Property(e => e.PdpaCsAId1)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("pdpa_cs_a_id1");
            entity.Property(e => e.PdpaCsAId2)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("pdpa_cs_a_id2");
            entity.Property(e => e.PdpaCsAId3)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("pdpa_cs_a_id3");
            entity.Property(e => e.PdpaCsALabel1)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("pdpa_cs_a_label1");
            entity.Property(e => e.PdpaCsALabel2)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("pdpa_cs_a_label2");
            entity.Property(e => e.PdpaCsALabel3)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("pdpa_cs_a_label3");
            entity.Property(e => e.PdpaCsQId1)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("pdpa_cs_q_id1");
            entity.Property(e => e.PdpaCsQId2)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("pdpa_cs_q_id2");
            entity.Property(e => e.PdpaCsQId3)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("pdpa_cs_q_id3");
            entity.Property(e => e.PoliticalPerson)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("political_person");
            entity.Property(e => e.PoliticalPosition)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("political_position");
            entity.Property(e => e.RedempBankAccountName)
                .HasMaxLength(200)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("redemp_bank_account_name");
            entity.Property(e => e.RedempBankAccountNo)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("redemp_bank_account_no");
            entity.Property(e => e.RedempBankBranchCode)
                .HasMaxLength(5)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("redemp_bank_branch_code");
            entity.Property(e => e.RedempBankCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("redemp_bank_code");
            entity.Property(e => e.RedempBankDefault)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("redemp_bank_default");
            entity.Property(e => e.RequestTime)
                .HasPrecision(15)
                .HasColumnName("request_time");
            entity.Property(e => e.ResiBuilding)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("resi_building");
            entity.Property(e => e.ResiCountry)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("resi_country");
            entity.Property(e => e.ResiDistrict)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("resi_district");
            entity.Property(e => e.ResiFloor)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("resi_floor");
            entity.Property(e => e.ResiMoo)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("resi_moo");
            entity.Property(e => e.ResiNo)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("resi_no");
            entity.Property(e => e.ResiPostal)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("resi_postal");
            entity.Property(e => e.ResiProvince)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("resi_province");
            entity.Property(e => e.ResiRoad)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("resi_road");
            entity.Property(e => e.ResiSoi)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("resi_soi");
            entity.Property(e => e.ResiSubDistrict)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("resi_sub_district");
            entity.Property(e => e.ResiVillage)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("resi_village");
            entity.Property(e => e.Spousecardnumber).HasColumnName("spousecardnumber");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SttUpdateStatus)
                .HasMaxLength(1)
                .HasDefaultValueSql("'N'::bpchar")
                .HasColumnName("stt_update_status");
            entity.Property(e => e.SttUpdateStatusCode)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("stt_update_status_code");
            entity.Property(e => e.SttUpdateStatusDatetime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("stt_update_status_datetime");
            entity.Property(e => e.SttUpdateStatusUser)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("stt_update_status_user");
            entity.Property(e => e.SubmittedTime)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("submitted_time");
            entity.Property(e => e.SuitCId1)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id1");
            entity.Property(e => e.SuitCId10)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id10");
            entity.Property(e => e.SuitCId11)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id11");
            entity.Property(e => e.SuitCId12)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id12");
            entity.Property(e => e.SuitCId13)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id13");
            entity.Property(e => e.SuitCId14)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id14");
            entity.Property(e => e.SuitCId2)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id2");
            entity.Property(e => e.SuitCId3)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id3");
            entity.Property(e => e.SuitCId41)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id41");
            entity.Property(e => e.SuitCId42)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id42");
            entity.Property(e => e.SuitCId43)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id43");
            entity.Property(e => e.SuitCId44)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id44");
            entity.Property(e => e.SuitCId5)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id5");
            entity.Property(e => e.SuitCId6)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id6");
            entity.Property(e => e.SuitCId7)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id7");
            entity.Property(e => e.SuitCId8)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id8");
            entity.Property(e => e.SuitCId9)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_id9");
            entity.Property(e => e.SuitCLabel1)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label1");
            entity.Property(e => e.SuitCLabel10)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label10");
            entity.Property(e => e.SuitCLabel11)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label11");
            entity.Property(e => e.SuitCLabel12)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label12");
            entity.Property(e => e.SuitCLabel13)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label13");
            entity.Property(e => e.SuitCLabel14)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label14");
            entity.Property(e => e.SuitCLabel2)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label2");
            entity.Property(e => e.SuitCLabel3)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label3");
            entity.Property(e => e.SuitCLabel41)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label41");
            entity.Property(e => e.SuitCLabel42)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label42");
            entity.Property(e => e.SuitCLabel43)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label43");
            entity.Property(e => e.SuitCLabel44)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label44");
            entity.Property(e => e.SuitCLabel5)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label5");
            entity.Property(e => e.SuitCLabel6)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label6");
            entity.Property(e => e.SuitCLabel7)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label7");
            entity.Property(e => e.SuitCLabel8)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label8");
            entity.Property(e => e.SuitCLabel9)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_c_label9");
            entity.Property(e => e.SuitCScore1)
                .HasPrecision(3)
                .HasColumnName("suit_c_score1");
            entity.Property(e => e.SuitCScore10)
                .HasPrecision(3)
                .HasColumnName("suit_c_score10");
            entity.Property(e => e.SuitCScore11)
                .HasPrecision(3)
                .HasColumnName("suit_c_score11");
            entity.Property(e => e.SuitCScore12)
                .HasPrecision(3)
                .HasColumnName("suit_c_score12");
            entity.Property(e => e.SuitCScore13)
                .HasPrecision(3)
                .HasColumnName("suit_c_score13");
            entity.Property(e => e.SuitCScore14)
                .HasPrecision(3)
                .HasColumnName("suit_c_score14");
            entity.Property(e => e.SuitCScore2)
                .HasPrecision(3)
                .HasColumnName("suit_c_score2");
            entity.Property(e => e.SuitCScore3)
                .HasPrecision(3)
                .HasColumnName("suit_c_score3");
            entity.Property(e => e.SuitCScore41)
                .HasPrecision(3)
                .HasColumnName("suit_c_score41");
            entity.Property(e => e.SuitCScore42)
                .HasPrecision(3)
                .HasColumnName("suit_c_score42");
            entity.Property(e => e.SuitCScore43)
                .HasPrecision(3)
                .HasColumnName("suit_c_score43");
            entity.Property(e => e.SuitCScore44)
                .HasPrecision(3)
                .HasColumnName("suit_c_score44");
            entity.Property(e => e.SuitCScore5)
                .HasPrecision(3)
                .HasColumnName("suit_c_score5");
            entity.Property(e => e.SuitCScore6)
                .HasPrecision(3)
                .HasColumnName("suit_c_score6");
            entity.Property(e => e.SuitCScore7)
                .HasPrecision(3)
                .HasColumnName("suit_c_score7");
            entity.Property(e => e.SuitCScore8)
                .HasPrecision(3)
                .HasColumnName("suit_c_score8");
            entity.Property(e => e.SuitCScore9)
                .HasPrecision(3)
                .HasColumnName("suit_c_score9");
            entity.Property(e => e.SuitQId1)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_id1");
            entity.Property(e => e.SuitQId10)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_id10");
            entity.Property(e => e.SuitQId11)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_id11");
            entity.Property(e => e.SuitQId12)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_id12");
            entity.Property(e => e.SuitQId13)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_id13");
            entity.Property(e => e.SuitQId14)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_id14");
            entity.Property(e => e.SuitQId2)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_id2");
            entity.Property(e => e.SuitQId3)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_id3");
            entity.Property(e => e.SuitQId4)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_id4");
            entity.Property(e => e.SuitQId5)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_id5");
            entity.Property(e => e.SuitQId6)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_id6");
            entity.Property(e => e.SuitQId7)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_id7");
            entity.Property(e => e.SuitQId8)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_id8");
            entity.Property(e => e.SuitQId9)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_id9");
            entity.Property(e => e.SuitQLebel1)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_lebel1");
            entity.Property(e => e.SuitQLebel10)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_lebel10");
            entity.Property(e => e.SuitQLebel11)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_lebel11");
            entity.Property(e => e.SuitQLebel12)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_lebel12");
            entity.Property(e => e.SuitQLebel13)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_lebel13");
            entity.Property(e => e.SuitQLebel14)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_lebel14");
            entity.Property(e => e.SuitQLebel2)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_lebel2");
            entity.Property(e => e.SuitQLebel3)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_lebel3");
            entity.Property(e => e.SuitQLebel4)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_lebel4");
            entity.Property(e => e.SuitQLebel5)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_lebel5");
            entity.Property(e => e.SuitQLebel6)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_lebel6");
            entity.Property(e => e.SuitQLebel7)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_lebel7");
            entity.Property(e => e.SuitQLebel8)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_lebel8");
            entity.Property(e => e.SuitQLebel9)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_q_lebel9");
            entity.Property(e => e.SuitRiskLevel)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("suit_risk_level");
            entity.Property(e => e.SuitSum)
                .HasPrecision(3)
                .HasColumnName("suit_sum");
            entity.Property(e => e.TFname)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("t_fname");
            entity.Property(e => e.TLname)
                .HasMaxLength(70)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("t_lname");
            entity.Property(e => e.TTitle)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("t_title");
            entity.Property(e => e.Taxid)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("taxid");
            entity.Property(e => e.Tel)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("tel");
            entity.Property(e => e.TraderId)
                .HasMaxLength(20)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("trader_id");
            entity.Property(e => e.TraderRefer)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("trader_refer");
            entity.Property(e => e.TransDate).HasColumnName("trans_date");
            entity.Property(e => e.Types)
                .HasMaxLength(200)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("types");
            entity.Property(e => e.UCid)
                .HasMaxLength(30)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("u_cid");
            entity.Property(e => e.UUserid)
                .HasPrecision(16)
                .HasColumnName("u_userid");
            entity.Property(e => e.VerifiType)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("verifi_type");
            entity.Property(e => e.Vipflag)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("vipflag");
            entity.Property(e => e.Vipquestion)
                .HasMaxLength(300)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("vipquestion");
            entity.Property(e => e.VipquestionCode)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("vipquestion_code");
            entity.Property(e => e.WorkBuilding)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("work_building");
            entity.Property(e => e.WorkCountry)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("work_country");
            entity.Property(e => e.WorkDistrict)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("work_district");
            entity.Property(e => e.WorkFloor)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("work_floor");
            entity.Property(e => e.WorkMoo)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("work_moo");
            entity.Property(e => e.WorkNo)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("work_no");
            entity.Property(e => e.WorkPostal)
                .HasMaxLength(10)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("work_postal");
            entity.Property(e => e.WorkProvince)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("work_province");
            entity.Property(e => e.WorkRoad)
                .HasMaxLength(350)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("work_road");
            entity.Property(e => e.WorkSoi)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("work_soi");
            entity.Property(e => e.WorkSubDistrict)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("work_sub_district");
            entity.Property(e => e.WorkVillage)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("work_village");
            entity.Property(e => e.ZipFile)
                .HasMaxLength(1)
                .HasDefaultValueSql("'N'::bpchar")
                .HasColumnName("zip_file");
            entity.Property(e => e.ZipPassword)
                .HasMaxLength(200)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("zip_password");
        });
        modelBuilder.HasSequence("eopen_sba_id_seq");
        modelBuilder.HasSequence("users_id_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
