import { middyfy } from '@libs/lambda';

import { SampleData } from '../get-sampling/handler';

const delay = (ms) => new Promise((res) => setTimeout(res, ms));

const run = async (event) => {
  const sampleBatch: SampleData[] = event;
  try {
    await Promise.all(
      sampleBatch.map(async (sample) => {
        await delay(sample.processingTimeMS);
        const dateTime = new Date();
        console.log(
          `Current Time: ${dateTime.toLocaleString()} (${
            sample.name
          } completed)`
        );
      })
    );

    return {
      statusCode: 200,
      body: JSON.stringify({ message: 'Batch processed successfully' }),
    };
  } catch (error) {
    return {
      statusCode: 400,
      body: JSON.stringify({ error: error }),
    };
  }
};

export const main = middyfy(run);
