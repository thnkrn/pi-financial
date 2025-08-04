import { PDFDocument } from '@cantoo/pdf-lib';
import { InternalCustomerByIdentificationNo } from '@libs/user-v2-api';
import { readFile } from 'fs';
import path from 'path';

export const companyLogoAttachment = {
  filename: 'companyLogo.jpg', // adjust filename as needed
  path: path.join('resources', 'atlas', 'assets', 'logo_pi.png'), // adjust path as needed
  cid: 'companyLogo', // Content-ID used in the HTML
};

export async function modifyAndEncryptPdf(inputPdf: Buffer, password: string) {
  // Use pdf-lib to create a new PDF and add content
  const pdfDoc = await PDFDocument.load(inputPdf, { updateMetadata: true });

  await pdfDoc.encrypt({
    userPassword: password,
  });
  pdfDoc.setTitle('');
  pdfDoc.setProducer('');
  pdfDoc.setCreator('');
  const pdfBytes = await pdfDoc.save({ useObjectStreams: false });

  return pdfBytes;
}

export async function mergePdf(pdfs: Buffer[]) {
  const mergedPdf = await PDFDocument.create();

  for (const pdf of pdfs) {
    const pdfDoc = await PDFDocument.load(pdf);
    const copiedPages = await mergedPdf.copyPages(
      pdfDoc,
      pdfDoc.getPageIndices()
    );
    copiedPages.forEach((page) => mergedPdf.addPage(page));
  }
  const pdfBytes = await mergedPdf.save({ useObjectStreams: false });

  return pdfBytes;
}

export function getPDFPassword(dateOfBirth: string): string {
  try {
    const dateOfBirthRemoveDash = dateOfBirth.replace(/-/g, '');
    // Format as DDMMYYYY
    const password = `${dateOfBirthRemoveDash.substring(
      6
    )}${dateOfBirthRemoveDash.substring(4, 6)}${dateOfBirthRemoveDash.substring(
      0,
      4
    )}`;
    return password;
  } catch (error) {
    console.error('Error in getPDFPassword:', error);
    throw error;
  }
}

export function readFileAsBuffer(filePath: string): Promise<Buffer | null> {
  return new Promise((resolve, reject) => {
    readFile(filePath, (err, data) => {
      if (err) {
        // If there's an error reading the file, reject the promise
        reject(err);
      } else {
        // If file is successfully read, resolve the promise with the data
        resolve(data);
      }
    });
  });
}

export function getFormattedDate(file: string) {
  // Extract the date string
  const dateString: string = file.split('_')[1];
  // Convert the date string to a Date object
  const year: number = parseInt(dateString.substring(0, 4), 10);
  const month: number = parseInt(dateString.substring(4), 10);
  const date: Date = new Date(year, month - 1); // Month is zero-based in JavaScript Date object
  // Define options for formatting
  const options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: 'long',
  };
  // Convert the date to a formatted string
  const formattedDateString: string = date.toLocaleString('en-US', options);
  return formattedDateString;
}

export function concatenateNames(
  customerData: InternalCustomerByIdentificationNo,
  marketingId: string
): string {
  const names: Set<string> = new Set(
    customerData.data.marketings
      .filter((marketing) =>
        marketingId == '' ? true : marketing.marketingId == marketingId
      )
      .flatMap((marketingGroup) =>
        marketingGroup.custCodes.map((custCodeGroup) => {
          const { firstnameTh, lastnameTh } = custCodeGroup.basicInfo.name;
          return `${firstnameTh} ${lastnameTh}`;
        })
      )
  );

  return Array.from(names).join('\nOR\n');
}

export const htmlEmailContent = `
  <!DOCTYPE html>
  <html lang="en">
  <head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Pi Securities Monthly Statement</title>
  </head>
  <body>
  <h1>Pi Securities PCL</h1>
  <p>
    เรียน   ท่านลูกค้าผู้มีอุปการคุณ <br>
    เรื่อง  Pi Securities Monthly Statement 
  </p>
  <p>
    บริษัทฯ ขอนำส่งรายงานแสดงยอดหลักทรัพย์คงเหลือ (Monthly Statement) ของท่าน ในรูปแบบ PDF <br>
    ท่านสามารถเรียกดูรายละเอียดได้โดย คลิกไฟล์ PDF <br>
    สำหรับรหัสที่ใช้เปิดเอกสารแนบ ตามรายละเอียดด้านล่าง
  </p>
  <p>
    บุคคลธรรมดา: วัน/เดือน/ปี ค.ศ. เกิดของท่าน ในรูปแบบ DDMMYYYY <br>
    นิติบุคคล: 5 หลักสุดท้ายของหมายเลขทะเบียนนิติบุคคล
  </p>
  <p>
    ( หากท่านไม่สามารถเปิดเอกสารที่แนบมาได้ กรุณาดาวน์โหลด Adobe Acrobat Reader ได้จาก  <a href="https://get.adobe.com/reader/">https://get.adobe.com/reader/</a> )
  </p>
  <p>
    หมายเหตุ: <br>
    - หากท่านมีการเปลี่ยนแปลงอีเมล์ กรุณาแจ้งบริษัทฯ เพื่อการจัดส่งที่ถูกต้อง <br>
    - กรุณาอย่าตอบกลับอีเมล์นี้
  </p>
  <p>
    สอบถามข้อมูลเพิ่มเติมได้ที่ ฝ่ายบริการลูกค้า โทร 02-205-7000
  </p>
  <p>
    ขอแสดงความนับถือ <br>
    บมจ. หลักทรัพย์ พาย จำกัด (มหาชน)
  </p>
  <img src="cid:companyLogo" alt="Company Logo">
  <hr>
  <p>
    Dear: Valued Client <br>
    Subject: Pi Securities Monthly Statement
  </p>
  <p>
    Attached please find your electronic monthly statement of account in PDF format. <br>
    To view the statement, please double click the attached PDF <br>
    To open the file with the security, please confirm the password as below
  </p>
  <p>
    For individual customer: Please specify your birthday as DDMMYYYY <br>
    For corporate account: Please specify the last 5 digits of your corporate registration number.
  </p>
  <p>
    (If you are unable to open the attached document, please download the latest version of Adobe Acrobat Reader from the following site, <a href="https://get.adobe.com/reader/">https://get.adobe.com/reader/</a>)
  </p>
  <p>
    - In case of you change your e-mail address, please notify us immediately. <br>
    - PLEASE DO NOT REPLY TO THIS MAIL.
  </p>
  <p>
    If you have any queries, please contact our Customer Services on 02-205-7000
  </p>
  <p>
    Best Regards, <br>
    Pi Securities PCL.
  </p>
  <img src="cid:companyLogo" alt="Company Logo">
  </body>
  </html>
`;
