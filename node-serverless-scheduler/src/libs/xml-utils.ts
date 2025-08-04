import { parseStringPromise } from 'xml2js';

export const parseXmlContent = async (xmlBody: string) => {
  try {
    return await parseStringPromise(xmlBody);
  } catch (error) {
    console.error('Failed to parse XML string');
    throw error;
  }
};
