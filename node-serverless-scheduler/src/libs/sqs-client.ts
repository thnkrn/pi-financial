import {
  DeleteQueueCommand,
  paginateListQueues,
  SQSClient,
} from '@aws-sdk/client-sqs';

const REGION = 'ap-southeast-1';
const sqsClient = new SQSClient({ region: REGION });

export async function listQueue() {
  const paginatedListQueues = paginateListQueues({ client: sqsClient }, {});
  const urls: string[] = [];
  for await (const page of paginatedListQueues) {
    const nextUrls = page.QueueUrls?.filter((qurl) => !!qurl) || [];
    urls.push(...nextUrls);
  }
  return urls;
}

export async function deleteQueue(queueUrl: string): Promise<void> {
  const command = new DeleteQueueCommand({ QueueUrl: queueUrl });

  const response = await sqsClient.send(command);

  if (response.$metadata.httpStatusCode !== 200) {
    console.log(`Unable To Delete SQS Queue: ${queueUrl}`);
  }
}
