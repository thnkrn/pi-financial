import fs from 'fs';
import { promisify } from 'util';

export const checkIfFileOrDirectoryExists = (path: string): boolean => {
  return fs.existsSync(path);
};

export const createFile = async (
  path: string,
  fileName: string,
  data: any,
): Promise<void> => {
  if (!checkIfFileOrDirectoryExists(path)) {
    fs.mkdirSync(path);
  }

  const writeFile = promisify(fs.writeFile);

  return await writeFile(`${path}/${fileName}`, data, 'utf8');
};

export const deleteFile = async (path: string): Promise<void> => {
  const unlink = promisify(fs.unlink);

  return await unlink(path);
};
