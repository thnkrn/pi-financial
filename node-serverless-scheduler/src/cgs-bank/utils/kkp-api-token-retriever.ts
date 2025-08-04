import kkpApi from '@cgs-bank/external-api-services/kkp-api/authentication';
import {
  getKKPApiToken,
  storeKKPApiToken,
} from '@cgs-bank/database-services/dynamodb-cached-services';

const functions = {
  retrieveApiKey: async (): Promise<string> => {
    try {
      console.log('Getting kkp api token');
      return await getKKPApiToken();
    } catch (error) {
      console.log(error);
      console.log('Start fetching new token');
      const resToken = await kkpApi.getToken();
      console.log('Resolve new token');
      try {
        await storeKKPApiToken(resToken.access_token);
        return resToken.access_token;
      } catch (error) {
        console.log('Store Error');
        console.log(error);
        throw error;
      }
    }
  },
};

export default functions;
