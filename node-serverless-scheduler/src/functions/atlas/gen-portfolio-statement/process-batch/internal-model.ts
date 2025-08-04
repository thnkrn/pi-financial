import { PortfolioBase } from 'db/portfolio-summary/models/PortfolioBase';
import { Model } from 'sequelize';
import { ProductBase } from './response-model';

export interface GroupedMarketingProductType {
  marketingId: string;
  productTypes: GroupedProductType[];
}

export interface GroupedProductType {
  exchangeMarketId: string;
  accountType: string;
  accountTypeCode: string;
  customerType: string;
  customerSubType: string;
  tradingAccounts: TradingAccount[];
}

export interface TradingAccount {
  custcode: string;
  tradingAccountNo: string;
}

export interface MarketingCustcode {
  marketingId: string;
  custcode: string;
}

export interface PortfolioItem<T extends Model<PortfolioBase>> {
  products: T[];
  dateKey?: Date; //null in case of empty product table or trading accout not available for those customer
}

export interface ProductItem {
  products: ProductBase[];
  dateKey?: Date;
  accountName: string;
}

export interface DateKeyItem<T> {
  items: T[];
  dateKey?: Date;
}

export interface GEActivityStatement {
  accountNumber: string;
  accountName: string;
  taxIdNumber: string;
  address: string;
  marketingOfficer: string;
  period: string;
}
export interface GEOutStandingCashBalance {
  currency: string;
  balance: number;
}
export interface GEStocksAndETF {
  symbol: string;
  currency: string;
  quantity: number;
  costPrice: number;
  closePrice: number;
  marketValue: number;
}
export interface GEDepositwithdraw {
  transactionDate: Date;
  transaction: string;
  currency: string;
  amount: number;
  fxRate: number;
  amountThb: number;
}
export interface GETrade {
  transactionDate: Date;
  symbol: string;
  side: string;
  currency: string;
  quantity: number;
  price: number;
  grossAmount: number;
  commission: number;
  vat: number;
  otherFee: number;
  withholdingTax: number;
  netAmount: number;
}
export interface GEDividend {
  transactionDate: Date;
  symbol: string;
  currency: string;
  quantity: number;
  dividendPerShare: number;
  amount: number;
  taxAmount: number;
  fxRate: number;
  netAmount: number;
}
