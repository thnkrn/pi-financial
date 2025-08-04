
export function base64ToBlob(base64String: string) {
    const parts = base64String.split(';');
    const raw = parts[1].split(',')[1];
  
    const contentTypeMatch = /^data:([A-Za-z-+/]+);base64,/.exec(base64String);
    const contentType = contentTypeMatch ? contentTypeMatch[1] : 'application/octet-stream';
  
    const byteCharacters = atob(raw);
    const byteArrays = [];
  
    for (let offset = 0; offset < byteCharacters.length; offset += 512) {
      const slice = byteCharacters.slice(offset, offset + 512);
  
      const byteNumbers = new Array(slice.length);
      for (let i = 0; i < slice.length; i++) {
        byteNumbers[i] = slice.charCodeAt(i);
      }
  
      const byteArray = new Uint8Array(byteNumbers);
      byteArrays.push(byteArray);
    }
  
    const blob = new Blob(byteArrays, { type: contentType });
    return blob;
  }

  export function toBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        if (typeof reader.result === 'string') {
          resolve(reader.result);
        } else {
          reject(new Error('Failed to convert file to base64.'));
        }
      };
      reader.onerror = () => {
        reject(new Error('Failed to read the file.'));
      };
    });
  }
