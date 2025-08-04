import { ProductType } from '@functions/atlas/gen-portfolio-statement/utils/etl-utils';
import { listObject } from '@libs/s3-utils';

export const GetS3ProductWithDateKeyPrefix = (
  product: string,
  dateKey: string
) => {
  return `${product}/date_key=${dateKey}/`;
};

export const ListPortfolioS3Files = async (
  product: ProductType,
  bucket: string,
  dateKey: string
) => {
  const prefix = GetS3ProductWithDateKeyPrefix(product, dateKey);
  const response = await listObject(bucket, prefix);

  if (!Array.isArray(response.Contents) || !response.Contents.length) {
    throw new Error(`File not found for ${prefix} in ${bucket}`);
  }

  const importData = response.Contents.filter(
    (c) => c.Key.startsWith(prefix) && c.Key.endsWith('.csv')
  ).map((c) => c.Key);

  if (
    importData === undefined ||
    !Array.isArray(importData) ||
    !importData.length
  ) {
    throw new Error(`File not found for ${prefix} in ${bucket}`);
  }

  console.info(`Found import data: ${prefix} in ${bucket}`);
  console.info(importData);

  return importData;
};
