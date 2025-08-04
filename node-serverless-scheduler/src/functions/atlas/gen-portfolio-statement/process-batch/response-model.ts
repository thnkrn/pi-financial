import {
  GEActivityStatement,
  GEDepositwithdraw,
  GEDividend,
  GEOutStandingCashBalance,
  GEStocksAndETF,
  GETrade,
} from './internal-model';

export interface CustomerStatementData {
  thaiId: string;
  customerName: string;
  customerAddress: string;
  isHighNetWorth: boolean; //true, Green theme
  consolidatedSummary: PortfolioSummary;
  exchangeRate: PortfolioExchangeRate[];
  marketings: MarketingCustcodeData[];
}

export interface MarketingCustcodeData {
  marketingId: string;
  marketingName: string;
  marketingPhoneNo: string;
  portfoliosummary: PortfolioSummary;
  custcodeProductMapping: CustcodeProductMapping[];

  balanceSummary: BalanceSummary[];
  privateFundSummary: PrivateFundSummary[];

  thaiEquity: PortfolioThaiEquity[];
  mutualFund: PortfolioMutualFund[];
  mutualFundOffshore: PortfolioMutualFund[];
  bond: PortfolioBond[];
  bondOffshore: PortfolioBondOffshore[];
  tfex: PortfolioTfex[];
  cash: PortfolioCash[];
  structuredProduct: PortfolioStructuredProduct[];
  structuredProductOnshore: PortfolioStructuredProduct[];
  globalEquityOtc: PortfolioGlobalEquityOtc[];
  globalEquity: PortfolioGlobalEquity[];
  alternativePrivateEquity: PortfolioAlternativePrivateEquity[];
  alternativePrivateCredit: PortfolioAlternativePrivateCredits[];
  privateFund: PortfolioPrivateFund[];
}

export interface PrivateFundSummary {
  custcode: string;
  accountName: string;
  nav: number;
}

export interface BalanceSummary {
  custcode: string;
  thaiEquityCash: number;
  thaiEquityCashBalance: number;
  thaiEquityCreditBalance: number;
  tfex: number;
  tfexEquityValue: number;
  tfexExcessEquity: number;
  structureProduct: number;
  globalEquity: number;
}

export interface CustcodeProductMapping {
  custcode: string;
  products: ProductAccount[];
}

export interface ProductAccount {
  accountName: string;
  accountNo: string;
}

export interface PortfolioExchangeRate {
  currency: string;
  exchangeRate: number;
  dateKey: Date;
}

export class PortfolioSummary {
  custcode: string[];
  allocationAmount: number;
  yearOnYear: number;
  y_0: number;
  y_1: number;
  y_2: number;
  y_3: number;
  m_0: number;
  m_1: number;
  m_2: number;
  m_3: number;
  m_4: number;
  m_5: number;
  m_6: number;
  m_7: number;
  m_8: number;
  m_9: number;
  m_10: number;
  m_11: number;
  summaryAsOf: string;
  portfolioAllocation: PortfolioAllocation[];
  latestYearIndex: number;
  latestMonthIndex: number;

  hasYearOrMonthValue(): boolean {
    // Check y_* properties
    for (let i = 0; i <= 3; i++) {
      const yearValue = this[`y_${i}`];
      if (yearValue !== undefined && yearValue !== null && yearValue !== 0) {
        return true;
      }
    }

    // Check m_* properties
    for (let i = 0; i <= 11; i++) {
      const monthValue = this[`m_${i}`];
      if (monthValue !== undefined && monthValue !== null && monthValue !== 0) {
        return true;
      }
    }

    return false;
  }
}

export class PortfolioAllocation {
  productName: string;
  percentAllocation: number;
  valueAllocation: number;
}

export interface ProductBase {
  custcode: string;
  accountNo: string;
  accountName: string;
}

export interface PortfolioThaiEquity extends ProductBase {
  custcode: string;
  accountNo: string;
  sharecode: string;
  unit: number;
  avgPrice: number;
  marketPrice: number;
  totalCost: number;
  marketValue: number;
  gainLoss: number;
  dateKey: Date;
  accountName: string;
}
export interface PortfolioMutualFund extends ProductBase {
  custcode: string;
  accountNo: string;
  fundCategory: string;
  amccode: string;
  fundName: string;
  navDate: Date;
  unit: number;
  avgNavCost: number;
  marketNav: number;
  totalCost: number;
  marketValue: number;
  gainLoss: number;
  dateKey: Date;
  accountName: string;
  currency: string;
}
export interface PortfolioBond extends ProductBase {
  custcode: string;
  accountNo: string;
  marketType: string;
  assetName: string;
  issuer: string;
  maturityDate?: string;
  tenureDays?: string;
  initialDate: Date;
  couponRate: string;
  totalCost: number;
  marketValue: number;
  dateKey: Date;
  accountName: string;
}

export interface PortfolioBondOffshore extends ProductBase {
  custcode: string;
  accountNo: string;
  marketType: string;
  assetName: string;
  issuer: string;
  maturityDate: Date;
  initialDate: Date;
  nextCallDate: Date;
  couponRate: number;
  units: number;
  currency: string;
  avgCost: number;
  totalCost: number;
  marketValueOriginalCurrency: number;
  marketValue: number;
  dateKey: Date;
  accountName: string;
}

export interface PortfolioTfex extends ProductBase {
  custcode: string;
  accountNo: string;
  sharecode: string;
  multiplier: number;
  unit: number;
  avgPrice: number;
  marketPrice: number;
  totalCost: number;
  marketValue: number;
  gainLoss: number;
  dateKey: Date;
  accountName: string;
}
export interface PortfolioCash extends ProductBase {
  custcode: string;
  accountNo: string;
  currency: string;
  currencyFullName: string;
  cashBalance: number;
  marketValue: number;
  dateKey: Date;
  accountName: string;
}
export interface PortfolioStructuredProduct extends ProductBase {
  custcode: string;
  accountNo: string;
  productType: string;
  issuer: string;
  note: string;
  underlying: string;
  tradeDate: Date;
  maturityDate: Date;
  tenor: number;
  capitalProtection: string;
  yield: number;
  originalCurrency: string;
  currency: string;
  notionalValue: number;
  marketValueOriginalCurrency: number;
  marketValue: number;
  dateKey: Date;
  accountName: string;
}
export interface PortfolioGlobalEquityOtc extends ProductBase {
  custcode: string;
  accountNo: string;
  sharecode: string;
  equityCategory: string;
  originalCurrency: string;
  currency: string;
  units: number;
  avgCost: number;
  marketPrice: number;
  totalCost: number;
  marketValue: number;
  marketValueOriginalCurrency: number;
  gainLoss: number;
  dateKey: Date;
  accountName: string;
}
export interface PortfolioGlobalEquity extends ProductBase {
  custcode: string;
  accountNo: string;
  sharecode: string;
  equityCategory: string;
  originalCurrency: string;
  currency: string;
  units: number;
  avgCost: number;
  marketPrice: number;
  totalCost: number;
  marketValue: number;
  marketValueOriginalCurrency: number;
  gainLoss: number;
  dateKey: Date;
  accountName: string;
}

export interface PortfolioAlternativePrivateEquity extends ProductBase {
  custcode: string;
  accountNo: string;
  sharecode: string;
  unit: number;
  avgPrice: number;
  marketPrice: number;
  totalCost: number;
  marketValue: number;
  gainLoss: number;
  dateKey: Date;
  accountName: string;
}

export interface PortfolioAlternativePrivateCredits extends ProductBase {
  custcode: string;
  accountNo: string;
  assetName: string;
  initialDate: Date;
  maturityDate: Date;
  couponRate: number;
  nextCouponDate: Date;
  initialValue: number;
  marketValue: number;
  dateKey: Date;
  accountName: string;
}

export interface PortfolioPrivateFund extends ProductBase {
  custcode: string;
  accountNo: string;
  tickercode: string;
  issuer: string;
  type: string;
  weight: string;
  unit: number;
  avgPrice: number;
  marketPrice: number;
  totalCost: number;
  marketValue: number;
  gainLoss: number;
  dateKey: Date;
  accountName: string;
}

export interface SNCashMovementCustcodeGroup {
  custcode: string;
  subAccount: string;
  dateKey: Date;
  customerName: string;
  snCurrency: SNCashMovementCurrencyGroup[];
}

export interface SNCashMovementCurrencyGroup {
  currency: string;
  currencyFullName: string;
  cashMovement: SNCashMovement[];
}

export interface SNCashMovement {
  custcode: string;
  accountNo: string;
  subAccount: string;
  transactionDate: Date;
  settlementDate: Date;
  transactionType: string;
  currency: string;
  amountDebit: number;
  amountCredit: number;
  balance: number;
  note: string;
  dateKey: Date;
}

export interface GlobalEquityGroup {
  activityStatement: GEActivityStatement;
  outstandingCashBalance: GEOutStandingCashBalance[];
  stocksAndETFs: GEStocksAndETF[];
  depositwithdraws: GEDepositwithdraw[];
  trades: GETrade[];
  dividends: GEDividend[];
}
