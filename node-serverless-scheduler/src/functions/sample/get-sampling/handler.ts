import { middyfy } from '@libs/lambda';

export interface SampleData {
  id: number;
  name: string;
  processingTimeMS: number;
}

const run = async (event) => {
  try {
    const samplingData: SampleData[] = [
      {
        id: 1,
        name: 'Test1',
        processingTimeMS: 1000,
      },
      {
        id: 2,
        name: 'Test2',
        processingTimeMS: 2000,
      },
      {
        id: 3,
        name: 'Test3',
        processingTimeMS: 1000,
      },
      {
        id: 4,
        name: 'Test4',
        processingTimeMS: 1000,
      },
      {
        id: 5,
        name: 'Test5',
        processingTimeMS: 1000,
      },
    ];

    return {
      statusCode: 200,
      body: batchArray(samplingData, Number(event.body.batchSize)),
    };
  } catch (error) {
    console.error('Error get customer data:', error);
    throw error;
  }
};

function batchArray<T>(array: T[], batchSize: number): T[][] {
  const batches: T[][] = [];
  for (let i = 0; i < array.length; i += batchSize) {
    batches.push(array.slice(i, i + batchSize));
  }
  return batches;
}

export const main = middyfy(run);
