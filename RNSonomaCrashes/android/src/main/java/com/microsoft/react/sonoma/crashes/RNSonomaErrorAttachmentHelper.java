package com.microsoft.sonoma.react.crashes;

import android.content.Context;

import com.microsoft.sonoma.crashes.model.ErrorReport;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;

public class RNSonomaErrorAttachmentHelper {
    private static final String CRASHES_DIRECTORY_NAME = "RNSonomaCrashes";
    private static final String ERROR_ATTACHMENTS_DIRECTORY_NAME = "TextAttachments";

    private static File getErrorAttachmentFile(Context context, ErrorReport report) {
        String filesDir = context.getFilesDir().getAbsolutePath();
        String crashesDir = new File(filesDir, CRASHES_DIRECTORY_NAME).getAbsolutePath();
        File errorAttachmentsDir = new File(crashesDir, ERROR_ATTACHMENTS_DIRECTORY_NAME);
        errorAttachmentsDir.mkdirs();
        return new File(errorAttachmentsDir, report.getId());
    }

    public static void saveTextAttachment(Context context, ErrorReport report, String text) throws IOException {
        File errorAttachmentFile = getErrorAttachmentFile(context, report);
        if (errorAttachmentFile.exists()) {
            errorAttachmentFile.delete();
        }

        PrintWriter out = null;
        try {
            out = new PrintWriter(errorAttachmentFile.getAbsolutePath());
            out.print(text);
        } finally {
            if (out != null) out.close();
        }
    }

    public static String getTextAttachment(Context context, ErrorReport report) throws IOException {
        FileInputStream fileInputStream = null;
        BufferedReader bufferedReader = null;

        File errorAttachmentFile = getErrorAttachmentFile(context, report);
        if (!errorAttachmentFile.exists()) {
            return null;
        }

        try {
            fileInputStream = new FileInputStream(errorAttachmentFile);
            bufferedReader = new BufferedReader(new InputStreamReader(fileInputStream));
            StringBuilder sb = new StringBuilder();
            String line = null;
            while ((line = bufferedReader.readLine()) != null) {
                sb.append(line).append("\n");
            }

            return sb.toString();
        } finally {
            if (bufferedReader != null) bufferedReader.close();
            if (fileInputStream != null) fileInputStream.close();
        }
    }

    public static boolean deleteTextAttachment(Context context, ErrorReport report) {
        File errorAttachmentFile = getErrorAttachmentFile(context, report);
        return errorAttachmentFile.exists() && errorAttachmentFile.delete();
    }
}
