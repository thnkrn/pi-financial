export const thaiPhoneNumberValidator = (value: string) => {
  const reg = new RegExp('^\\d+$');
  if (!reg.test(value) || value.length != 10) {
    throw 'Invalid Mobile Number';
  }
  return;
};

export const emailFormatValidator = (value: string) => {
  const reg = new RegExp('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$');
  if (!reg.test(value)) {
    throw 'Invalid Email Format';
  }
  return;
};
