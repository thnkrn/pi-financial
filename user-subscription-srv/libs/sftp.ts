import Logger from '@utils/datadog-utils';
import Client from 'ssh2-sftp-client';

export class SFTPClient {
  private readonly host;
  private readonly port;
  private readonly user;
  private readonly password;
  private readonly client = new Client();

  constructor(
    host: string = process.env.SFTP_HOST,
    port: string = process.env.SFTP_PORT,
    user: string = process.env.SFTP_USERNAME,
    password: string = process.env.SFTP_PASSWORD,
  ) {
    this.host = host;
    this.port = port;
    this.user = user;
    this.password = password;
  }
  async uploadSFTPFile(rawData: string, fileName: string) {
    try {
      await this.client.connect({
        host: process.env.SFTP_HOST,
        port: Number(process.env.SFTP_PORT),
        username: process.env.SFTP_USERNAME,
        password: process.env.SFTP_PASSWORD,
      });
      await this.client.put(Buffer.from(rawData, 'utf-8'), fileName);
      await this.client.end();
      return;
    } catch (e) {
      Logger.error(`uploadSFTPFile: ${e}`);
    }
  }

  toDateString = (timestamp) => {
    const date = new Date(timestamp);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');

    return `${year}${month}${day}`;
  };

  async downloadSFTPFile(
    fileName: string,
  ): Promise<{ data: any; date: string }> {
    try {
      await this.client.connect({
        host: process.env.INCENTIVE_SFTP_HOST,
        port: Number(process.env.INCENTIVE_SFTP_PORT),
        username: process.env.INCENTIVE_SFTP_USERNAME,
        password: process.env.INCENTIVE_SFTP_PASSWORD,
      });
      const remoteStat = await this.client.stat(fileName);
      const date = this.toDateString(remoteStat.modifyTime);

      const data = await this.client.get(fileName);
      await this.client.end();
      return { data, date };
    } catch (e) {
      Logger.error(`uploadSFTPFile: ${e}`);
    }
  }
}
