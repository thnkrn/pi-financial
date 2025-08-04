import { Op } from 'sequelize';
import {
  UserPaymentGateway,
  UserPaymentGatewayOutput,
} from '@cgs-bank/models/user-paymeny-gateway';

export const getUserByUsername = async (
  name: string
): Promise<UserPaymentGatewayOutput> => {
  const users = await UserPaymentGateway.findAll({
    where: {
      ...{ username: { [Op.eq]: name } },
    },
  });
  if (!users) {
    // @todo throw custom error
    throw new Error('User not found');
  }
  const output: UserPaymentGatewayOutput = {
    id: users[0].id,
    username: users[0].username,
    password: users[0].password,

    createdAt: users[0].createdAt,
    updatedAt: users[0].updatedAt,
  };
  return output;
};
