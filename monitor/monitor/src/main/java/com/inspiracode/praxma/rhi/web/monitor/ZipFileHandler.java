package com.inspiracode.praxma.rhi.web.monitor;

import java.io.BufferedInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;

public class ZipFileHandler {
	
	public byte[] getEntry(String zipFileLocation, String filePath) throws IOException {
		
		ZipFile zipFile = new ZipFile(zipFileLocation);
		
		System.out.println("Retrieving zip file item: " + filePath);
		ZipEntry entry = zipFile.getEntry(filePath);
		int entrySize = (int) entry.getSize();
		InputStream is = null;
		BufferedInputStream bis = null;
		try {
			is = zipFile.getInputStream(entry);
			bis = new BufferedInputStream(is);
			byte[] finalByteArray = new byte[entrySize];

			int bufferSize = 2048;
			byte[] buffer = new byte[2048];
			int chunkSize = 0;
			int bytesRead = 0;

			while (true) {
				// Read chunk to buffer
				chunkSize = bis.read(buffer, 0, bufferSize); // read() returns
																// the number of
																// bytes read
				if (chunkSize == -1) {
					// read() returns -1 if the end of the stream has been
					// reached
					break;
				}

				// Write that chunk to the finalByteArray
				// System.arraycopy(src, srcPos, dest, destPos, length)
				System.arraycopy(buffer, 0, finalByteArray, bytesRead,
						chunkSize);

				bytesRead += chunkSize;
			}

			System.err.println("Entry size: " + finalByteArray.length);
			return finalByteArray;
		} catch (IOException e) {
			System.err.println("No zip entry found at: " + filePath);
			return null;
		} finally {
			if(bis!=null)
				bis.close(); // close BufferedInputStream
			
			if(is!=null)
				is.close();
			
			zipFile.close();
		}
	}
}
