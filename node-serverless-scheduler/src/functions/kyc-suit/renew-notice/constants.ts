export enum NotificationType {
  KYC = 'Kyc',
  SUIT = 'Suitability',
}

export const NotificationTemplateId = {
  upcoming: {
    [NotificationType.KYC]: 1,
    [NotificationType.SUIT]: 4,
  },
  lapsed: {
    [NotificationType.KYC]: 3,
    [NotificationType.SUIT]: 2,
  },
};
