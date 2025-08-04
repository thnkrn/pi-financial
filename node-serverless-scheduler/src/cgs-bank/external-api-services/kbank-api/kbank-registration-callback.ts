export class KbankRegistrationCallback {
  externalReference: string;
  payerShortName: string;
  espaId: string;
  accountNo: string;
  userEmailMatching: string;
  userMobileMatching: string;
  idMatching: string;
  timestamp: string;
  returnStatus: string;
  returnCode: string;
  returnMessage: string;

  constructor(rawData: string) {
    const message = rawData.split('=')[1].replace(/\+/g, ' ');

    this.externalReference = message.substring(0, 20).trim();
    this.payerShortName = message.substring(20, 50).trim();
    this.espaId = message.substring(50, 150).trim();
    this.accountNo = message.substring(150, 170).trim();
    this.userEmailMatching = message.substring(170, 171);
    this.userMobileMatching = message.substring(171, 172);
    this.idMatching = message.substring(172, 173);
    this.timestamp = message.substring(173, 187).trim();
    this.returnStatus = message.substring(187, 188);
    this.returnCode = message.substring(188, 193).trim();
    this.returnMessage = message.substring(193, 449).trim();
  }
}
