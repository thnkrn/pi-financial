import {
  CreateTopicCommand,
  CreateTopicCommandOutput,
  PublishCommand,
  PublishCommandOutput,
  SNSClient,
} from '@aws-sdk/client-sns';

const REGION = 'ap-southeast-1';
const snsClient = new SNSClient({ region: REGION });

export const sendMessageToSNS = async ({
  topicName,
  data,
  groupId,
}): Promise<PublishCommandOutput> => {
  const createTopicCmd = new CreateTopicCommand({
    Name: topicName,
    Attributes: {
      FifoTopic: 'true',
      ContentBasedDeduplication: 'true',
    },
  });
  const resp: CreateTopicCommandOutput = await snsClient.send(createTopicCmd);

  if (!resp.TopicArn) {
    return new Promise((_, reject) => {
      reject(new Error(`Could not create Topic: ${topicName}`));
    });
  }

  const publishCmd = new PublishCommand({
    Message: JSON.stringify(data),
    MessageGroupId: groupId,
    TopicArn: resp.TopicArn,
  });

  return snsClient.send(publishCmd);
};

export default snsClient;
