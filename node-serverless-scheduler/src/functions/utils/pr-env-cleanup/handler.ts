import { middyfy } from '@libs/lambda';
import {
  formatJSONResponse,
  ValidatedEventAPIGatewayProxyEvent,
} from '@libs/api-gateway';
import schema from '@functions/utils/pr-env-cleanup/schema';
import {
  deleteTopic,
  getSubscriptionByTopic,
  listTopic,
  unsubscribe,
} from '@libs/sns-client';
import { deleteQueue, listQueue } from '@libs/sqs-client';

const _handle = async (topicName: string, queueName: string) => {
  console.log(`Main 1. Cleaning SNS Topic`);
  await _cleanUpSNS(topicName);
  console.log(`Main 2. Cleaning SQS Topic`);
  await _cleanUpSQS(queueName);
};

const _cleanUpSNS = async (topicName: string): Promise<void> => {
  console.log(`CleanupSNS - 1. Finding SNS Topic contain word: ${topicName}`);
  const topicList = await _getFilteredTopic(topicName);
  console.log(`CleanupSNS - 2. SNS Topic Found: ${topicList}`);
  console.log(`CleanupSNS - 3. Deleting SNS Topic`);
  await _deleteSnsTopics(topicList);
  console.log(`CleanupSNS - 4. SNS Topic Deleted`);
};

const _cleanUpSQS = async (queueName: string): Promise<void> => {
  console.log(`CleanupSQS - 1. Finding SQS Queue contain word: ${queueName}`);
  const queueLists = await _getFilteredQueue(queueName);
  console.log(`CleanupSQS - 2. SQS Queue Found: ${queueLists}`);
  console.log(`CleanupSQS - 3. Deleting SQS Queue`);
  await _deleteSqsQueues(queueLists);
  console.log(`CleanupSQS - 4. SQS Queue Deleted`);
};

const _getFilteredQueue = async (queueName: string): Promise<string[]> =>
  (await listQueue()).filter((name) => name.includes(queueName));

const _getFilteredTopic = async (topicName: string): Promise<string[]> =>
  (await listTopic()).filter((name) => name.includes(topicName));

const _deleteSnsTopics = async (topicList: string[]): Promise<void> => {
  for (const topic of topicList) {
    console.log(
      `DeleteSNSTopics - 1. GetSubscriptionByTopic for Topic ${topic}`
    );
    const subscriptionList = await getSubscriptionByTopic(topic);
    console.log(
      `DeleteSNSTopics - 2. GetSubscriptionByTopic for Topic ${topic}, SubscriptionList: ${subscriptionList}`
    );
    for (const subscription of subscriptionList) {
      console.log(
        `DeleteSNSTopics - 1.1. Unsubscribe for Topic ${topic}, Subscription: ${subscription}`
      );
      await unsubscribe(subscription);
      console.log(
        `DeleteSNSTopics - 1.2. Unsubscribe for Topic ${topic}, Subscription: ${subscription} Completed`
      );
    }
    console.log(`DeleteSNSTopics - 2. Delete Topic ${topic}`);
    await deleteTopic(topic);
    console.log(`DeleteSNSTopics - 3. Delete Topic ${topic} Completed`);
  }
};

const _deleteSqsQueues = async (queueList: string[]): Promise<void> => {
  for (const queue of queueList) {
    await deleteQueue(queue);
  }
};

const _run: ValidatedEventAPIGatewayProxyEvent<typeof schema> = async (
  event
) => {
  await _handle(event.body.topicName, event.body.queueName);

  return formatJSONResponse({
    status: 'done',
  });
};

const prEnvCleanUp = async () => {
  const topicName = '-pr-';
  const queueName = '-pr-';
  await _handle(topicName, queueName);
};

const run = middyfy(_run);

export { run, prEnvCleanUp };
