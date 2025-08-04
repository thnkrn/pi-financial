import { v4 as guid } from 'uuid';
import { getSnsParam, sendMessageToSNS } from '@libs/sns-client';

export type CreateNotificationTags = {
  icon: string;
  payload: string;
  backgroundColor: string;
  textColor: string;
  type: string;
};

export type CreateNotificationRequest = {
  cmsTemplateId: string | number;
  userId: string;
  type: string;
  shouldStoreDb: boolean;
  isPush: boolean;
  payloadTitle?: string[];
  payloadBody?: string[];
  url?: string;
  tags?: CreateNotificationTags[];
};

export type DeleteNotificationRequest = {
  userId: string;
  type: string;
};

////////////////////////////////////////////////////////////////////////////////

const createNotificationParams = getSnsParam(
  'Pi.NotificationService.Application.Commands',
  'CreateNotification'
);
const deleteNotificationParams = getSnsParam(
  'Pi.NotificationService.Application.Commands',
  'MarkNotificationInactive'
);

export async function createNotification(
  payload: CreateNotificationRequest
): Promise<void> {
  const messageId = guid();
  await sendMessageToSNS(createNotificationParams.topicName, {
    messageId,
    messageType: [createNotificationParams.urn],
    message: { ...payload, ticketId: messageId },
  });
}

export async function deleteNotification(
  payload: DeleteNotificationRequest
): Promise<void> {
  const messageId = guid();
  await sendMessageToSNS(deleteNotificationParams.topicName, {
    messageId,
    messageType: [deleteNotificationParams.urn],
    message: { ...payload, ticketId: messageId },
  });
}
