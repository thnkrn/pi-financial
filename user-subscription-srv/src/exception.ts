import { HttpException, HttpStatus } from '@nestjs/common';

export class UndefineUserId extends HttpException {
  constructor() {
    super('Undefine UserId', HttpStatus.UNAUTHORIZED);
  }
}

export class GuestAccountError extends HttpException {
  constructor() {
    super(
      "Your wallet isn't ready, please complete your wallet creation first.",
      HttpStatus.ACCEPTED,
    );
  }
}

export class CustcodeNotFound extends HttpException {
  constructor() {
    super('Cannot find customer code from user-id', HttpStatus.UNAUTHORIZED);
  }
}

export class MultipleCustcode extends HttpException {
  constructor() {
    super('Multiple CustomerCode', HttpStatus.ACCEPTED);
  }
}
