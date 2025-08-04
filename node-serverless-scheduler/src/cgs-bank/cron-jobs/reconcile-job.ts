import {
  getTimestamp,
  nowBangkok,
  TimestampFormat,
} from '@cgs-bank/utils/timestamp';
import kkpJob from '@cgs-bank/cron-jobs/kkp-jobs';
import AWS from 'aws-sdk';

const functions = {
  start: async (type: string) => {
    console.log('----- Start Equity Reconcile Job -----');
    console.log(getTimestamp(TimestampFormat.Extra, nowBangkok()));

    await kkpJob.start();
    console.log('----- KKP JOB COMPLETED -----');

    const params = {
      FunctionName: `cgs-reconcile-srv-${process.env.stage}-startReconcileJob`,
      Payload: Buffer.from(
        JSON.stringify({
          jobName: 'bankDepositWithdraw',
          type,
        })
      ),
    };

    const result = await new AWS.Lambda().invoke(params).promise();
    console.log(result);
    console.log('----- End Equity Reconcile Job -----');
    console.log(getTimestamp(TimestampFormat.Extra, nowBangkok()));
  },
};

export default functions;
