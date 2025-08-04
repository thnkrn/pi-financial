import { Injectable, Logger } from '@nestjs/common';
import { SFTPClient } from '@libs/sftp';
import s3 from '@libs/s3';

@Injectable()
export class IncentiveService {
  private readonly logger = new Logger(IncentiveService.name);
  private readonly sftpClient = new SFTPClient();

  async backupIncentive(): Promise<boolean> {
    const sftpPath = '/home/incentive/upload';
    const filenames = [
      'CUSVAL.TXT',
      'ADJCUSVAL.TXT',
      'CUSTFEXPRDMT4.CSV',
      'CUSTFEXPRD.CSV',
      'Comm_TFEX.csv',
      'Comm_Equity.csv',
    ];

    const s3Client = s3();

    try {
      for (const filename of filenames) {
        const remoteFilePath = `${sftpPath}/${filename}`;
        const { data, date } = await this.sftpClient.downloadSFTPFile(
          remoteFilePath,
        );
        this.logger.log(
          `File ${filename} downloaded successfully from ${remoteFilePath}`,
        );
        const bucket = `${process.env.INCENTIVE_BUCKET_NAME}`;
        const key = `${date}/${filename}`;

        if (Buffer.isBuffer(data)) {
          await s3Client.parallelUpload(bucket, key, data);
        } else {
          const bufferFromString: Buffer = Buffer.from(data);
          await s3Client.parallelUpload(bucket, key, bufferFromString);
        }
      }
    } catch (err) {
      this.logger.error(`Error downloading files: ${err.message}`);
    }

    return true;
  }
}
