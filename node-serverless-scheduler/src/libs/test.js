const {
  SQSClient,
  SendMessageCommand,
  GetQueueUrlCommand,
} = require('@aws-sdk/client-sqs');
const {
  SNSClient,
  PublishCommand,
  CreateTopicCommand,
} = require('@aws-sdk/client-sns');

const config = {
  region: 'us-east-1',
  endpoint: 'http://127.0.0.1:4566',
};

const sqsClient = new SQSClient(config);
const snsClient = new SNSClient(config);

const AWS = require('aws-sdk');
AWS.config.update(config);
const SNS = new AWS.SNS();

async function sendMessageToSNS(
  topicName,
  data,
  { groupId, deduplicationId } = {}
) {
  const topicCmd = new CreateTopicCommand({ Name: topicName });
  const resp = await snsClient.send(topicCmd);
  if (!resp.TopicArn) {
    throw new Error(`Could not find QueueUrl from QueueName: ${topicName}`);
  }
  console.log(topicName, resp.TopicArn);

  const cmd = new PublishCommand({
    TopicArn: resp.TopicArn,
    //   TargetArn?: string;
    //   PhoneNumber?: string;
    Message: encap(JSON.stringify(data)),
    //   Subject?: string;
    //   MessageStructure?: string;
    //   MessageAttributes?: Record<string, MessageAttributeValue>;
    MessageDeduplicationId: deduplicationId,
    MessageGroup: groupId,
  });

  return snsClient.send(cmd);
}

function getArn(topicName, region) {
  return `arn:aws:sns:us-east-1:000000000000:${topicName}`;
}

// sendMessageToSQS(
//   'notification-srv-local-oneport-order-event',
//   JSON.stringify({
//     AccountId: 1,
//     Symbol: 'notification-srv-local-oneport-order-event',
//     Side: 0,
//     Volume: 6969,
//   })
// );
sendMessageToSNS('Pi_Order_IntegrationEvents-OneportOrderRejectedEvent', {
  AccountId: '1',
  Symbol: 'Pi_Order_IntegrationEvents-OneportOrderRejectedEvent',
  Side: 'Buy',
  Volume: 6969.0,
});

function encap(data) {
  return `
  {
    "messageType": [
        "urn:message:Pi.Order.IntegrationEvents:OneportOrderRejectedEvent"
    ],
    "message": ${data},
}`;
}
