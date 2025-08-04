import chromium from '@sparticuz/chromium';
import path from 'path';
import puppeteer, { Browser, JSHandle, Page } from 'puppeteer';

export const generatePdfBuffer = async (
  html: any,
  dimensions: { width: any; height: any },
  waitUntil: (page: Page) => Promise<JSHandle<false> | JSHandle<true>>
) => {
  let browser: Browser;
  if (process.env.DEBUG_LOCAL === 'true') {
    //chromium not working with Mac
    browser = await puppeteer.launch({
      args: chromium.args,
      defaultViewport: chromium.defaultViewport,
      headless: chromium.headless as boolean,
      timeout: 360000,
    });
  } else {
    browser = await puppeteer.launch({
      args: chromium.args,
      defaultViewport: chromium.defaultViewport,
      executablePath: await chromium.executablePath(
        path.join('resources', 'atlas', 'chromium-v119.0.2-pack')
      ),
      headless: chromium.headless as boolean,
      timeout: 360000,
    });
  }

  const page = await browser.newPage();
  await page.setContent(html, { waitUntil: 'domcontentloaded' });

  // Wait for Chart.js chart to be fully rendered
  await waitUntil(page);

  const pdfBuffer = await page.pdf({
    width: dimensions.width,
    height: dimensions.height,
    printBackground: true,
  });
  await browser.close();
  return pdfBuffer;
};
