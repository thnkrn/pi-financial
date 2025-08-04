export const defaultNull = (str: string) => {
  return str !== null && str !== '' ? str : null;
};

export const stringNotNone = (str) => {
  const value = defaultNull(str);
  return value !== null && value?.toLowerCase() !== 'none' ? value : null;
};

export const stringToFloat = (str: string) => {
  if (defaultNull(str) === null) {
    return null;
  }
  return parseFloat(str);
};

export const stringToNumber = (str: string) => {
  if (defaultNull(str) === null) {
    return null;
  }
  return Number(str);
};

export const stringToDate = (str: string): Date | null => {
  if (defaultNull(str) === null) {
    return null;
  }
  return new Date(str);
};
