import { isDevelopmentOrStaging } from './env-helper';

const EMAIL_DOMAIN = '@pi.financial';

export const getEmailFromName = (name: string): string => {
  if (isDevelopmentOrStaging()) {
    return process.env.ORACLE_REPORT_EMAIL_TO;
  }
  const nameSplits = name.trim().toLowerCase().split(/\s+/);
  if (nameSplits.length < 2) {
    return '';
  }
  const firstName = nameSplits[nameSplits.length - 2];
  const lastName = nameSplits[nameSplits.length - 1].substring(0, 2);
  return `${firstName}.${lastName}${EMAIL_DOMAIN}`;
};

export const getFirstName = (inputString: string): string => {
  const prefixPattern = /^(\*\S+|MR\.|นางสาว|นาง|นาย|น\.ส\.)/;
  const cleanedName = inputString.replace(prefixPattern, '').trim();
  const firstName = cleanedName.split(/\s+/)[0];
  return firstName;
};
