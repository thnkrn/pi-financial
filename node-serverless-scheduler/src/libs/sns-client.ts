import {
  CreateTopicCommand,
  DeleteTopicCommand,
  paginateListSubscriptionsByTopic,
  paginateListTopics,
  PublishCommand,
  PublishCommandOutput,
  SNSClient,
  UnsubscribeCommand,
} from '@aws-sdk/client-sns';

// For more info https://masstransit.io/documentation/configuration/integrations/serialization
interface MasstransitPayload<PT> {
  messageId: string; // GUID
  correlationId?: string; // GUID
  requestId?: string; // GUID
  initiatorId?: string; // GUID
  conversationId?: string; // GUID
  sourceAddress?: string; // URI
  destinationAddress?: string; // URI
  responseAddress?: string; // URI
  faultAddress?: string; // URI
  expirationTime?: Date; // ISO-8601
  sentTime?: Date; // ISO-8601
  messageType: string[]; // urn:message:[namespace]:[classname], used for deserialization
  message: PT; // Actual payload object
}

const REGION = 'ap-southeast-1';
const snsClient = new SNSClient({ region: REGION });

export function getSnsParam(
  namespace: string,
  className: string
): { topicName: string; urn: string } {
  return {
    topicName: `${namespace.replace(/\./g, '_')}-${className}`,
    urn: `urn:message:${namespace}:${className}`,
  };
}

export async function sendMessageToSNS<PT = unknown>(
  topicName: string,
  payload: Partial<MasstransitPayload<PT>> &
    Pick<MasstransitPayload<PT>, 'message' | 'messageType'>,
  {
    groupId,
    deduplicationId,
  }: { groupId?: string; deduplicationId?: string } = {}
): Promise<PublishCommandOutput> {
  const topicCmd = new CreateTopicCommand({ Name: topicName });
  const resp = await snsClient.send(topicCmd);
  if (!resp.TopicArn) {
    throw new Error(`Could not get TopicArn from QueueName: ${topicName}`);
  }

  const cmd = new PublishCommand({
    TopicArn: resp.TopicArn,
    Message: JSON.stringify(payload),
    MessageDeduplicationId: deduplicationId,
    MessageGroupId: groupId,
  });

  return snsClient.send(cmd);
}

export async function listTopic() {
  const paginatedListTopics = paginateListTopics({ client: snsClient }, {});
  const topics: string[] = [];
  for await (const page of paginatedListTopics) {
    const nextTopics =
      page.Topics?.filter((t) => !!t.TopicArn).map((t) => t.TopicArn) || [];
    topics.push(...nextTopics);
  }
  return topics;
}

export async function deleteTopic(topicName: string) {
  const response = await snsClient.send(
    new DeleteTopicCommand({ TopicArn: topicName })
  );

  if (response.$metadata.httpStatusCode !== 200) {
    console.log(`Unable To Delete SNS Topic: ${topicName}`);
  }
}

export async function getSubscriptionByTopic(topicName: string) {
  const paginatedListSubscriptionByTopic = paginateListSubscriptionsByTopic(
    { client: snsClient },
    { TopicArn: topicName }
  );
  const subscriptions: string[] = [];
  for await (const page of paginatedListSubscriptionByTopic) {
    console.log(`getSubscriptionByTopic: ${page}`);
    const nextSubscription =
      page.Subscriptions?.filter((s) => !!s.SubscriptionArn).map(
        (s) => s.SubscriptionArn
      ) || [];
    subscriptions.push(...nextSubscription);
  }

  return subscriptions;
}

export async function unsubscribe(subscribeArn: string) {
  const response = await snsClient.send(
    new UnsubscribeCommand({ SubscriptionArn: subscribeArn })
  );

  if (response.$metadata.httpStatusCode !== 200) {
    console.log(`Unable To Unsubscribe: ${subscribeArn}`);
  }
}
