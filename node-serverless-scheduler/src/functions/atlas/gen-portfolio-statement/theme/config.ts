import { readFileSync } from 'fs';
import path from 'path';

export enum ImageFormat {
  png = 'png',
  svg = 'svg+xml',
  jpg = 'jpeg',
}

function getImageUrl(imageName: string, imageFormat: ImageFormat): string {
  const imagePath = path.join('resources', 'atlas', 'assets', imageName);
  let imageBuffer: Buffer | string;

  if (imageFormat === ImageFormat.svg) {
    const file = readFileSync(imagePath, 'utf-8');
    imageBuffer = Buffer.from(file, 'utf-8');
  } else {
    imageBuffer = readFileSync(imagePath);
  }

  const imageBase64 = imageBuffer.toString('base64');
  const imageUrl = `data:image/${imageFormat};base64,${imageBase64}`;
  return imageUrl;
}

export const themeConfig = {
  green: {
    color: '#0c5b59',
    background:
      'linear-gradient(180deg, #07403e 21.89%, #062d2c 132.84%), linear-gradient(0deg, rgba(0, 0, 0, 0.2), rgba(0, 0, 0, 0.2))',
    backgroundLigthGreen: '#ced8d8',
    colorDarkGreen: '#0c5b59',
    largeCard: '#FFFFFF4D',
    smallCard: '#FFFFFFCC',
    largeCardTextColor: '#fff',
    headerImage: getImageUrl('logo-one.svg', ImageFormat.svg),
    innerImage: getImageUrl('inner-image.png', ImageFormat.png),
    innerThemeImage: getImageUrl('inner-theme-image.png', ImageFormat.png),
    greenProfolio:
      'linear-gradient(180deg, #07403e 21.89%, #062d2c 132.84%),linear-gradient(0deg, rgba(0, 0, 0, 0.2), rgba(0, 0, 0, 0.2))',
  },
  blue: {
    color: '#004f84',
    background: '#004f84',
    backgroundLigthGreen: '#ced8d8',
    colorDarkGreen: '#000',
    largeCard: '#FFFFFFCC',
    smallCard: '#FFFFFFCC',
    largeCardTextColor: '#000',
    blackText: '#000',
    headerImage: getImageUrl('main-logo.svg', ImageFormat.svg),
    innerImage: getImageUrl('pi_logo.png', ImageFormat.png),
    innerThemeImage: getImageUrl('pi_logo.png', ImageFormat.png),
    greenProfolio: '#004F84',
  },
};

export const themeSNConfig = {
  headerImage: getImageUrl('pi_banner_logo.png', ImageFormat.png),
};

export const themeGEConfig = {
  headerImage: getImageUrl('pi-logo-green.svg', ImageFormat.svg),
};
