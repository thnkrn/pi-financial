import {
  MySqlSCBDDRegister,
  SCBDDRegisterInput,
} from '@cgs-bank/models/database/scb-dd-register';
import {
  MySqlSCBDDRegisterResult,
  SCBDDRegisterResultInput,
} from '@cgs-bank/models/database/scb-dd-register-result';

export const createScbDDRegister = async (
  data: SCBDDRegisterInput
): Promise<MySqlSCBDDRegister> => {
  return await MySqlSCBDDRegister.create(data);
};

export const createScbDDRegisterResult = async (
  data: SCBDDRegisterResultInput
): Promise<MySqlSCBDDRegisterResult> => {
  return await MySqlSCBDDRegisterResult.create(data);
};
