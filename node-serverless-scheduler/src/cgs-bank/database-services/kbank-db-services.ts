import {
  MySqlKBANKDDRegister,
  KBANKDDRegisterInput,
} from '@cgs-bank/models/database/kbank-dd-register';
import {
  MySqlKBANKDDRegisterResult,
  KBANKDDRegisterResultInput,
} from '@cgs-bank/models/database/kbank-dd-register-result';

export const createKBANKDDRegister = async (
  data: KBANKDDRegisterInput
): Promise<MySqlKBANKDDRegister> => {
  return await MySqlKBANKDDRegister.create(data);
};

export const createKBANKDDRegisterResult = async (
  data: KBANKDDRegisterResultInput
): Promise<MySqlKBANKDDRegisterResult> => {
  return await MySqlKBANKDDRegisterResult.create(data);
};
