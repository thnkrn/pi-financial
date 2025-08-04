import { middyfy } from '@libs/lambda';
import { getAccessibility, getSecretValue } from '@libs/secrets-manager-client';
import { SecretManagerType } from '@libs/db-utils';
import JSONbig from 'json-bigint';

const BATCH_SIZE = 10;

const JSONBigInt = JSONbig({
  storeAsString: true,
  useNativeBigInt: false,
});

interface CmeProduct {
  productGuid: string;
  productGuidInt: string;
}

interface CmeApiResponse {
  _embedded: {
    products: CmeProduct[];
  };
}

interface ProductResult {
  productCode: string;
  productGuidInt: string;
}

const run = async (event) => {
  if (!event.configs) throw new Error('No configs provided');

  const cmeSecret = await getSecretValue(
    'cme-marketdata',
    getAccessibility(SecretManagerType.Secret)
  );
  const CME_REF_API_HOST = cmeSecret['CME_REF_API_HOST'];

  const access_token = event.access_token;
  const product_codes: string[] =
    event.configs.map((q) => q.product_code) || [];

  if (!CME_REF_API_HOST)
    throw new Error('CME_REF_API_HOST missing from secret');
  if (!access_token) throw new Error('access_token missing from input');
  if (!Array.isArray(product_codes) || product_codes.length === 0) {
    throw new Error('product_codes must be a non-empty array');
  }

  const productResults: ProductResult[] = [];

  await batchRun(product_codes, BATCH_SIZE, async (symbol) => {
    const url = `${CME_REF_API_HOST.replace(
      /\/$/,
      ''
    )}/refdata/v3/products?symbol=${encodeURIComponent(
      symbol
    )}&securityType=FUT`;

    console.info(`URL: ${url}`);

    const res = await fetch(url, {
      method: 'GET',
      headers: {
        Authorization: `Bearer ${access_token}`,
        Accept: 'application/json',
      },
    });

    if (!res.ok) {
      throw new Error(
        `Failed to fetch product info for symbol ${symbol}: ${res.status} ${res.statusText}`
      );
    }

    const responseText = await res.text();
    const data = JSONBigInt.parse(responseText) as CmeApiResponse;

    const products = data._embedded?.products || [];
    for (const prod of products) {
      if (prod.productGuidInt) {
        console.info(
          `productGuidInt push to lambda response: ${prod.productGuidInt}`
        );
        productResults.push({
          productCode: symbol,
          productGuidInt: prod.productGuidInt,
        });
      }
    }
  });

  return {
    products: productResults,
  };
};

async function batchRun<T>(
  inputs: T[],
  limit: number,
  fn: (input: T) => Promise<void>
) {
  let idx = 0;
  while (idx < inputs.length) {
    const batch = inputs.slice(idx, idx + limit);
    await Promise.all(batch.map(fn));
    idx += limit;
  }
}

export const main = middyfy(run);
