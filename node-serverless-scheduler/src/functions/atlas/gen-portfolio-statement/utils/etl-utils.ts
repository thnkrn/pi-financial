import { initModel, ProductModelMap } from 'db/portfolio-summary/models';
import { Sequelize, Transaction } from 'sequelize';
import { ListPortfolioS3Files } from 'src/common/atlas';

type CreateQueryFunction = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => string;

// Map product type to S3 file paths
export enum ProductType {
  Summary = 'summary',
  Cash = 'cash',
  ExchangeRate = 'exchange_rate',
  ThaiEquity = 'thai_equity',
  Tfex = 'tfex',
  TfexSummary = 'tfex_summary',
  MutualFunds = 'mutual_funds',
  MutualFundsDividend = 'mutual_funds_dividend',
  Debentures = 'debentures',
  OffshoreDebentures = 'offshore_debentures',
  GlobalEquity = 'global_equity',
  GlobalEquityOtc = 'global_equity_otc',
  GlobalEquityDepositwithdraw = 'global_equity_depositwithdraw',
  GlobalEquityTrade = 'global_equity_trade',
  GlobalEquityDividend = 'global_equity_dividend',
  StructuredProducts = 'structured_products',
  StructuredProductsOnshore = 'onshore_structured_products',
  StructureNoteCashMovement = 'structure_note_cash_movement',
}

export const importDataFromFiles = async (
  sequelize: Sequelize,
  bucket: string,
  dateKey: string,
  product: ProductType,
  queryFn: CreateQueryFunction
) => {
  let importData = [];
  try {
    importData = await ListPortfolioS3Files(product, bucket, dateKey);
  } catch (error) {
    console.warn(`Error list ${dateKey} ${bucket} ${product}, (${error})`);
  }

  const result = importData.map(async (file) => {
    try {
      await sequelize.query(queryFn(bucket, file, dateKey));
    } catch (error) {
      console.error(`Error import ${product} files:`, error);
      throw error;
    }
  });

  return Promise.all(result);
};

export const incrementalImportDataFromFiles = async (
  sequelize: Sequelize,
  bucket: string,
  dateKey: string,
  product: ProductType,
  queryFn: CreateQueryFunction
) => {
  let importData = [];
  try {
    importData = await ListPortfolioS3Files(product, bucket, dateKey);
  } catch (error) {
    console.warn(`Error list ${dateKey} ${bucket} ${product}`);
    return;
  }

  initModel(sequelize, product);
  const transaction = await sequelize.transaction();
  try {
    await cleanUpDataByDateKey(product, dateKey, transaction);
    await Promise.all(
      importData.map(async (file) => {
        try {
          await sequelize.query(queryFn(bucket, file, dateKey), {
            transaction,
            benchmark: true,
            logging: (_: string, timing?: number) => {
              console.log(`Executing insert: '${product}' in ${timing}ms`);
            },
          });
        } catch (error) {
          console.error(`Error import ${product} files:`, error);
          throw error;
        }
      })
    );
    await transaction.commit();
  } catch (error) {
    console.error(`Error importing ${product} files:`, error);
    await transaction.rollback();
    throw error;
  }
};

const cleanUpDataByDateKey = async (
  product: ProductType,
  dateKey: string,
  transaction: Transaction
) => {
  const model = ProductModelMap[product];

  await model.destroy({
    where: { dateKey: Date.parse(dateKey) },
    transaction,
    benchmark: true,
    logging: (sql: string, timing?: number) => {
      console.log(`Executing delete: '${sql}' in ${timing}ms`);
    },
  });
};

export const getImportQuery = (product: ProductType) => {
  switch (product) {
    case ProductType.Summary:
      return importSummaryQuery;
    case ProductType.Cash:
      return importCashQuery;
    case ProductType.ExchangeRate:
      return importExchangeRateQuery;
    case ProductType.ThaiEquity:
      return importThaiEquityQuery;
    case ProductType.Tfex:
      return importTfexQuery;
    case ProductType.TfexSummary:
      return importTfexSummaryQuery;
    case ProductType.MutualFunds:
      return importMutualFundQuery;
    case ProductType.MutualFundsDividend:
      return importMutualFundDividendQuery;
    case ProductType.Debentures:
      return importBondQuery;
    case ProductType.OffshoreDebentures:
      return importBondOffshoreQuery;
    case ProductType.GlobalEquity:
      return importGlobalEquityQuery;
    case ProductType.GlobalEquityOtc:
      return importGlobalEquityOtcQuery;
    case ProductType.GlobalEquityDepositwithdraw:
      return importGlobalEquityDepositwithdrawQuery;
    case ProductType.GlobalEquityTrade:
      return importGlobalEquityTradeQuery;
    case ProductType.GlobalEquityDividend:
      return importGlobalEquityDividendQuery;
    case ProductType.StructuredProducts:
      return importStructuredProductQuery;
    case ProductType.StructuredProductsOnshore:
      return importStructuredProductOnshoreQuery;
    case ProductType.StructureNoteCashMovement:
      return importStructureNoteCashMovementQuery;
  }
};

export const importCashQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_cash_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_account, @n_xchgmkt, @n_custtype, @n_subtype, @n_custacct, @n_acctcode, @n_currency, @n_fait_currency, @n_cash_balance)
            SET date_key = '${dateKey}', created_at = NOW(),
            currency = NULLIF (@n_currency, ''),
            cash_balance = NULLIF (@n_cash_balance, ''),
            trading_account_no = NULLIF (@n_account, ''),
            exchange_market_id = NULLIF (@n_xchgmkt, ''),
            customer_type = NULLIF (@n_custtype, ''),
            customer_sub_type = NULLIF (@n_subtype, ''),
            account_type = NULLIF (@n_custacct, ''),
            account_type_code = NULLIF (@n_acctcode, '');
          `;
};

export const importBondQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_bond_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_account, @n_xchgmkt, @n_custtype, @n_subtype, @n_custacct, @n_acctcode, @n_market, @n_categories, @n_transaction, @n_initial_date, @n_maturity_date, @n_issuer, @n_sharecode, @n_coupon_rate, @n_units, @n_total_cost, @n_market_value)
            SET date_key = '${dateKey}', created_at = NOW(),
            market_type = NULLIF (@n_categories, ''),
            asset_name = NULLIF (@n_sharecode, ''),
            issuer = NULLIF (@n_issuer, ''),
            maturity_date = NULLIF (@n_maturity_date, ''),
            initial_date = NULLIF (@n_initial_date, ''),
            coupon_rate = NULLIF (@n_coupon_rate, ''),
            total_cost = NULLIF (@n_total_cost, ''),
            market_value = NULLIF (@n_market_value, ''),
            trading_account_no = NULLIF (@n_account, ''),
            exchange_market_id = NULLIF (@n_xchgmkt, ''),
            customer_type = NULLIF (@n_custtype, ''),
            customer_sub_type = NULLIF (@n_subtype, ''),
            account_type = NULLIF (@n_custacct, ''),
            account_type_code = NULLIF (@n_acctcode, '');
          `;
};

export const importBondOffshoreQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_bond_offshore_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_account, @n_xchgmkt, @n_custtype, @n_subtype, @n_custacct, @n_acctcode, @n_market, @n_categories, @n_transaction, @n_initial_date, @n_maturity_date, @n_issuer, @n_sharecode, @n_coupon_rate, @n_units, @n_total_cost, @n_market_value, @n_next_call_date, @n_avg_cost, @n_currency)
            SET date_key = '${dateKey}', created_at = NOW(),
            market_type = NULLIF (@n_categories, ''),
            asset_name = NULLIF (@n_sharecode, ''),
            issuer = NULLIF (@n_issuer, ''),
            maturity_date = NULLIF (@n_maturity_date, ''),
            initial_date = NULLIF (@n_initial_date, ''),
            coupon_rate = NULLIF (@n_coupon_rate, ''),
            total_cost = NULLIF (@n_total_cost, ''),
            market_value = NULLIF (@n_market_value, ''),
            trading_account_no = NULLIF (@n_account, ''),
            exchange_market_id = NULLIF (@n_xchgmkt, ''),
            customer_type = NULLIF (@n_custtype, ''),
            customer_sub_type = NULLIF (@n_subtype, ''),
            account_type = NULLIF (@n_custacct, ''),
            account_type_code = NULLIF (@n_acctcode, ''),
            units = NULLIF (@n_units, ''),
            next_call_date = NULLIF (@n_next_call_date, ''),
            avg_cost = NULLIF (@n_avg_cost, ''),
            currency = NULLIF (@n_currency, '');
          `;
};

export const importExchangeRateQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_exchange_rate_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (currency, exchange_rate)
            SET date_key = '${dateKey}', created_at = NOW();
          `;
};

export const importGlobalEquityOtcQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_global_equity_otc_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_account, @n_xchgmkt, @n_custtype, @n_subtype, @n_custacct, @n_acctcode, @n_sharecode, @n_currency, @n_stock_exchange_markets, @n_units, @n_avg_cost, @n_market_price, @n_total_cost, @n_market_value, @n_gain_loss)
            SET date_key = '${dateKey}', created_at = NOW(),
            sharecode = NULLIF (@n_sharecode, ''),
            currency = NULLIF (@n_currency, ''),
            units = NULLIF (@n_units, ''),
            avg_cost = NULLIF (@n_avg_cost, ''),
            market_price = NULLIF (@n_market_price, ''),
            total_cost = NULLIF (@n_total_cost, ''),
            market_value = NULLIF (@n_market_value, ''),
            gain_loss = NULLIF (@n_gain_loss, ''),
            trading_account_no = NULLIF (@n_account, ''),
            exchange_market_id = NULLIF (@n_xchgmkt, ''),
            customer_type = NULLIF (@n_custtype, ''),
            customer_sub_type = NULLIF (@n_subtype, ''),
            account_type = NULLIF (@n_custacct, ''),
            account_type_code = NULLIF (@n_acctcode, ''),
            stock_exchange_markets = NULLIF (@n_stock_exchange_markets, '');
          `;
};

export const importGlobalEquityQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_global_equity_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_account, @n_xchgmkt, @n_custtype, @n_subtype, @n_custacct, @n_acctcode, @n_sharecode, @n_currency, @n_stock_exchange_markets, @n_units, @n_avg_cost, @n_market_price, @n_total_cost, @n_market_value, @n_gain_loss)
            SET date_key = '${dateKey}', created_at = NOW(),
            sharecode = NULLIF (@n_sharecode, ''),
            currency = NULLIF (@n_currency, ''),
            units = NULLIF (@n_units, ''),
            avg_cost = NULLIF (@n_avg_cost, ''),
            market_price = NULLIF (@n_market_price, ''),
            total_cost = NULLIF (@n_total_cost, ''),
            market_value = NULLIF (@n_market_value, ''),
            gain_loss = NULLIF (@n_gain_loss, ''),
            trading_account_no = NULLIF (@n_account, ''),
            exchange_market_id = NULLIF (@n_xchgmkt, ''),
            customer_type = NULLIF (@n_custtype, ''),
            customer_sub_type = NULLIF (@n_subtype, ''),
            account_type = NULLIF (@n_custacct, ''),
            account_type_code = NULLIF (@n_acctcode, ''),
            stock_exchange_markets = NULLIF (@n_stock_exchange_markets, '');
          `;
};

export const importMutualFundQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_mutual_fund_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_account, @n_xchgmkt, @n_custtype, @n_subtype, @n_custacct, @n_acctcode, @n_fund_category, @n_amccode, @n_fund_name, @n_nav_date, @n_unit, @n_avg_nav_cost, @n_market_nav, @n_total_cost, @n_market_value, @n_gain_loss, @n_currency)
            SET date_key = '${dateKey}', created_at = NOW(),
            fund_category = NULLIF (@n_fund_category, ''),
            amccode = NULLIF (@n_amccode, ''),
            fund_name = NULLIF (@n_fund_name, ''),
            nav_date = NULLIF (@n_nav_date, ''),
            unit = NULLIF (@n_unit, ''),
            avg_nav_cost = NULLIF (@n_avg_nav_cost, ''),
            market_nav = NULLIF (@n_market_nav, ''),
            total_cost = NULLIF (@n_total_cost, ''),
            market_value = NULLIF (@n_market_value, ''),
            gain_loss = NULLIF (@n_gain_loss, ''),
            currency = NULLIF (@n_currency, ''),
            trading_account_no = NULLIF (@n_account, ''),
            exchange_market_id = NULLIF (@n_xchgmkt, ''),
            customer_type = NULLIF (@n_custtype, ''),
            customer_sub_type = NULLIF (@n_subtype, ''),
            account_type = NULLIF (@n_custacct, ''),
            account_type_code = NULLIF (@n_acctcode, '');
          `;
};

export const importMutualFundDividendQuery = (
  bucket: string,
  file: string,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_mutual_fund_dividend_daily_transaction
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (@n_paymentdate, custcode, @n_account, @n_fund_code, @n_amccode, @n_unit, @n_dividendrate, @n_dividendamount, @n_witholdingtax, @n_dividendamountnet, @n_paymenttype_description, @n_bankname, @n_bankaccount, @n_bookcloseddate)
            SET date_key = '${dateKey}',
            id = UUID(),
            created_at = NOW(),
            payment_date = NULLIF (@n_paymentdate, ''),
            book_closed_date = NULLIF (@n_bookcloseddate, ''),
            trading_account_no = NULLIF (@n_account, ''),
            fund_code = NULLIF (@n_fund_code, ''),
            amccode = NULLIF (@n_amccode, ''),
            unit = NULLIF (@n_unit, ''),
            dividend_rate = NULLIF (@n_dividendrate, ''),
            dividend_amount = NULLIF (@n_dividendamount, ''),
            witholding_tax = NULLIF (@n_witholdingtax, ''),
            dividend_amount_net = NULLIF (@n_dividendamountnet, ''),
            payment_type_description = NULLIF (@n_paymenttype_description, ''),
            bank_name = NULLIF (@n_bankname, ''),
            bank_account = CASE
              WHEN @n_bankaccount IS NULL OR @n_bankaccount = '' THEN ''
              ELSE CONCAT('xxx', RIGHT(@n_bankaccount, 4))
            END;
          `;
};

export const importStructuredProductQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_structured_product_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_account, @n_xchgmkt, @n_custtype, @n_subtype, @n_custacct, @n_acctcode, @n_product_type, @n_issuer, @n_note, @n_underlying, @n_trade_date, @n_maturity_date, @n_tenor, @n_capital_protection, @n_yield, @n_currency, @n_exchange_rate, @n_notional_value, @n_market_value)
            SET date_key = '${dateKey}', created_at = NOW(),
            product_type = NULLIF (@n_product_type, ''),
            issuer = NULLIF (@n_issuer, ''),
            note = NULLIF (@n_note, ''),
            underlying = NULLIF (@n_underlying, ''),
            trade_date = NULLIF (@n_trade_date, ''),
            maturity_date = NULLIF (@n_maturity_date, ''),
            tenor = NULLIF (@n_tenor, ''),
            capital_protection = NULLIF (@n_capital_protection, ''),
            yield = NULLIF (@n_yield, ''),
            currency = NULLIF (@n_currency, ''),
            exchange_rate = NULLIF (@n_exchange_rate, ''),
            notional_value = NULLIF (@n_notional_value, ''),
            market_value = NULLIF (@n_market_value, ''),
            trading_account_no = NULLIF (@n_account, ''),
            exchange_market_id = NULLIF (@n_xchgmkt, ''),
            customer_type = NULLIF (@n_custtype, ''),
            customer_sub_type = NULLIF (@n_subtype, ''),
            account_type = NULLIF (@n_custacct, ''),
            account_type_code = NULLIF (@n_acctcode, '');
          `;
};

export const importStructuredProductOnshoreQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_structured_product_onshore_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_account, @n_xchgmkt, @n_custtype, @n_subtype, @n_custacct, @n_acctcode, @n_product_type, @n_issuer, @n_note, @n_underlying, @n_trade_date, @n_maturity_date, @n_tenor, @n_capital_protection, @n_yield, @n_currency, @n_exchange_rate, @n_notional_value, @n_market_value)
            SET date_key = '${dateKey}', created_at = NOW(),
            product_type = NULLIF (@n_product_type, ''),
            issuer = NULLIF (@n_issuer, ''),
            note = NULLIF (@n_note, ''),
            underlying = NULLIF (@n_underlying, ''),
            trade_date = NULLIF (@n_trade_date, ''),
            maturity_date = NULLIF (@n_maturity_date, ''),
            tenor = NULLIF (@n_tenor, ''),
            capital_protection = NULLIF (@n_capital_protection, ''),
            yield = NULLIF (@n_yield, ''),
            currency = NULLIF (@n_currency, ''),
            exchange_rate = NULLIF (@n_exchange_rate, ''),
            notional_value = NULLIF (@n_notional_value, ''),
            market_value = NULLIF (@n_market_value, ''),
            trading_account_no = NULLIF (@n_account, ''),
            exchange_market_id = NULLIF (@n_xchgmkt, ''),
            customer_type = NULLIF (@n_custtype, ''),
            customer_sub_type = NULLIF (@n_subtype, ''),
            account_type = NULLIF (@n_custacct, ''),
            account_type_code = NULLIF (@n_acctcode, '');
          `;
};

export const importSummaryQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_summary_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_mktid, @n_account, @n_xchgmkt, @n_custtype, @n_subtype, @n_custacct, @n_acctcode, @n_y_0, @n_y_1, @n_y_2, @n_y_3, @n_m_0, @n_m_1, @n_m_2, @n_m_3, @n_m_4, @n_m_5, @n_m_6, @n_m_7, @n_m_8, @n_m_9, @n_m_10, @n_m_11, @n_as_of_date, @n_exchange_rate_as_of)
            SET date_key = '${dateKey}', created_at = NOW(),
            mktid = NULLIF (@n_mktid, ''),
            y_0 = NULLIF (@n_y_0, ''),
            y_1 = NULLIF (@n_y_1, ''),
            y_2 = NULLIF (@n_y_2, ''),
            y_3 = NULLIF (@n_y_3, ''),
            m_0 = NULLIF (@n_m_0, ''),
            m_1 = NULLIF (@n_m_1, ''),
            m_2 = NULLIF (@n_m_2, ''),
            m_3 = NULLIF (@n_m_3, ''),
            m_4 = NULLIF (@n_m_4, ''),
            m_5 = NULLIF (@n_m_5, ''),
            m_6 = NULLIF (@n_m_6, ''),
            m_7 = NULLIF (@n_m_7, ''),
            m_8 = NULLIF (@n_m_8, ''),
            m_9 = NULLIF (@n_m_9, ''),
            m_10 = NULLIF (@n_m_10, ''),
            m_11 = NULLIF (@n_m_11, ''),
            trading_account_no = NULLIF (@n_account, ''),
            exchange_market_id = NULLIF (@n_xchgmkt, ''),
            customer_type = NULLIF (@n_custtype, ''),
            customer_sub_type = NULLIF (@n_subtype, ''),
            account_type = NULLIF (@n_custacct, ''),
            account_type_code = NULLIF (@n_acctcode, ''),
            as_of_date = NULLIF (@n_as_of_date, ''),
            exchange_rate_as_of = NULLIF (@n_exchange_rate_as_of, '');
          `;
};

export const importTfexQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_tfex_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_account, @n_xchgmkt, @n_custtype, @n_subtype, @n_custacct, @n_acctcode, @n_sharecode, @n_multiplier, @n_unit, @n_avg_price, @n_market_price, @n_total_cost, @n_market_value, @n_gain_loss)
            SET date_key = '${dateKey}', created_at = NOW(),
            sharecode = NULLIF (@n_sharecode, ''),
            multiplier = NULLIF (@n_multiplier, ''),
            unit = NULLIF (@n_unit, ''),
            avg_price = NULLIF (@n_avg_price, ''),
            market_price = NULLIF (@n_market_price, ''),
            total_cost = NULLIF (@n_total_cost, ''),
            market_value = NULLIF (@n_market_value, ''),
            gain_loss = NULLIF (@n_gain_loss, ''),
            trading_account_no = NULLIF (@n_account, ''),
            exchange_market_id = NULLIF (@n_xchgmkt, ''),
            customer_type = NULLIF (@n_custtype, ''),
            customer_sub_type = NULLIF (@n_subtype, ''),
            account_type = NULLIF (@n_custacct, ''),
            account_type_code = NULLIF (@n_acctcode, '');
          `;
};

export const importThaiEquityQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_thai_equity_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_account, @n_xchgmkt, @n_custtype, @n_subtype, @n_custacct, @n_acctcode, @n_sharecode, @n_unit, @n_avg_price, @n_market_price, @n_total_cost, @n_market_value, @n_gain_loss)
            SET date_key = '${dateKey}', created_at = NOW(),
            sharecode = NULLIF (@n_sharecode, ''),
            unit = NULLIF (@n_unit, ''),
            avg_price = NULLIF (@n_avg_price, ''),
            market_price = NULLIF (@n_market_price, ''),
            total_cost = NULLIF (@n_total_cost, ''),
            market_value = NULLIF (@n_market_value, ''),
            gain_loss = NULLIF (@n_gain_loss, ''),
            trading_account_no = NULLIF (@n_account, ''),
            exchange_market_id = NULLIF (@n_xchgmkt, ''),
            customer_type = NULLIF (@n_custtype, ''),
            customer_sub_type = NULLIF (@n_subtype, ''),
            account_type = NULLIF (@n_custacct, ''),
            account_type_code = NULLIF (@n_acctcode, '');
          `;
};

export const importTfexSummaryQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_tfex_summary_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_account, @n_xchgmkt, @n_custtype, @n_subtype, @n_custacct, @n_acctcode, @n_equity, @n_excess_equity)
            SET date_key = '${dateKey}', created_at = NOW(),
            equity = NULLIF (@n_equity, ''),
            excess_equity = NULLIF (@n_excess_equity, ''),
            trading_account_no = NULLIF (@n_account, ''),
            exchange_market_id = NULLIF (@n_xchgmkt, ''),
            customer_type = NULLIF (@n_custtype, ''),
            customer_sub_type = NULLIF (@n_subtype, ''),
            account_type = NULLIF (@n_custacct, ''),
            account_type_code = NULLIF (@n_acctcode, '');
          `;
};

export const importStructureNoteCashMovementQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE structure_note_cash_movement
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_account, @n_xchgmkt, @n_custtype, @n_subtype, @n_custacct, @n_acctcode, @n_subacct, @n_transaction_date, @n_settlement_date, @n_transaction_type, @n_currency, @n_amount, @n_note, @n_description)
            SET date_key = '${dateKey}', created_at = NOW(),
            sub_account = NULLIF (@n_subacct, ''),
            transaction_date = NULLIF (@n_transaction_date, ''),
            settlement_date = NULLIF (@n_settlement_date, ''),
            transaction_type = NULLIF (@n_transaction_type, ''),
            currency = NULLIF (@n_currency, ''),
            amount = NULLIF (@n_amount, ''),
            note = NULLIF (@n_note, ''),
            description = NULLIF (@n_description, ''),  
            trading_account_no = NULLIF (@n_account, ''),
            exchange_market_id = NULLIF (@n_xchgmkt, ''),
            customer_type = NULLIF (@n_custtype, ''),
            customer_sub_type = NULLIF (@n_subtype, ''),
            account_type = NULLIF (@n_custacct, ''),
            account_type_code = NULLIF (@n_acctcode, '');
          `;
};

export const importGlobalEquityDepositwithdrawQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_global_equity_depositwithdraw_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (@n_type, custcode, @n_account, @n_currency, @n_fx_rate, @n_amount_usd, @n_amount_thb)
            SET id = UUID(), date_key = '${dateKey}', created_at = NOW(),
            trading_account_no = @n_account,
            type = NULLIF (@n_type, ''),
            fx_rate = NULLIF (@n_fx_rate, ''),
            currency = NULLIF (@n_currency, ''),
            amount_usd = NULLIF (@n_amount_usd, ''),
            amount_thb = NULLIF (@n_amount_thb, '');
          `;
};

export const importGlobalEquityTradeQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_global_equity_trade_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_account, @n_xchgmkt, @n_sharecode, @n_side, @n_currency, @n_units, @n_avg_price, @n_gross_amount, @n_commission_before_vat_usd, @n_vat_amount, @n_other_fees, @n_wh_tax, @n_net_amount, @n_exchange_rate, @n_net_amount_thb)
            SET id = UUID(), date_key = '${dateKey}', created_at = NOW(),
            trading_account_no = @n_account,
            exchange_market_id = NULLIF (@n_xchgmkt, ''),
            sharecode = NULLIF (@n_sharecode, ''),
            side = NULLIF (@n_side, ''),
            currency = NULLIF (@n_currency, ''),
            units = NULLIF (@n_units, ''),
            avg_price = NULLIF (@n_avg_price, ''),
            gross_amount = NULLIF (@n_gross_amount, ''),
            commission_before_vat_usd = NULLIF (@n_commission_before_vat_usd, ''),
            vat_amount = NULLIF (@n_vat_amount, ''),
            other_fees = NULLIF (@n_other_fees, ''),
            wh_tax = NULLIF (@n_wh_tax, ''),
            net_amount = NULLIF (@n_net_amount, ''),
            exchange_rate = NULLIF (@n_exchange_rate, ''),
            net_amount_thb = NULLIF (@n_net_amount_thb, '');
          `;
};

export const importGlobalEquityDividendQuery = (
  bucket: string,
  file: ProductType,
  dateKey: string
) => {
  return `
          LOAD DATA FROM S3 's3://${bucket}/${file}' 
            INTO TABLE portfolio_global_equity_dividend_daily_snapshot
            FIELDS TERMINATED BY ',' ENCLOSED BY '"'
            LINES TERMINATED BY '\\n'
            IGNORE 1 LINES
            (custcode, @n_account, @n_sharecode, @n_currency, @n_units, @n_dividen_per_share, @n_amount, @n_tax_amount, @n_net_amount, @n_fx_rate, @n_net_amount_usd)
            SET id = UUID(), date_key = '${dateKey}', created_at = NOW(),
            trading_account_no = @n_account,
            sharecode = NULLIF (@n_sharecode, ''),
            currency = NULLIF (@n_currency, ''),
            units = NULLIF (@n_units, ''),
            dividen_per_share = NULLIF (@n_dividen_per_share, ''),
            amount = NULLIF (@n_amount, ''),
            tax_amount = NULLIF (@n_tax_amount, ''),
            net_amount = NULLIF (@n_net_amount, ''),
            fx_rate = NULLIF (@n_fx_rate, ''),
            net_amount_usd = NULLIF (@n_net_amount_usd, '');
          `;
};
