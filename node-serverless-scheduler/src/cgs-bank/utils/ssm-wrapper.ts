import { GetParameterCommand, SSMClient } from '@aws-sdk/client-ssm';
import { GetParameterCommandInput } from '@aws-sdk/client-ssm/dist-types/commands/GetParameterCommand';

const client = new SSMClient({ region: 'ap-southeast-1' });

export class SsmWrapper {
  private readonly _key: string;
  private _value: string;

  constructor(key: string, value?: string) {
    this._key = key;
    this._value = value ?? '';
  }

  async getValue() {
    console.log('Get Value');
    if (this._value.length == 0) {
      console.log('Fetching Value');
      const input: GetParameterCommandInput = {
        Name: this._key,
        WithDecryption: true,
      };
      console.log('Input');
      console.log(input);
      const command = new GetParameterCommand(input);
      console.log('Command');
      console.log(command);
      const response = await client.send(command);
      this._value = response.Parameter.Value;
      console.log(response);
    }

    return this._value;
  }
}
