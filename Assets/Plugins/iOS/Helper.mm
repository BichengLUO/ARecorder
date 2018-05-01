//
//  VideoCaptureViewController.m
//  AVFoundation
//
//  Created by Sylvanus on 4/20/18.
//  Copyright © 2018 Sylvanus. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <AVFoundation/AVFoundation.h>
#import <Photos/Photos.h>

BOOL isRecording;

CGSize size;
int frame_x, frame_y, frame_w, frame_h;
AVCaptureSession *session;

AVAssetWriter *assetWriter;
AVAssetWriterInput *assetWriterInput;
AVAssetWriterInputPixelBufferAdaptor *adaptor;
NSString *outputFilePath;
CMTime lastTimestamp;

extern "C" {
    
    void setupAssetWriter(char *videoPath) {
        NSLog(@"configuring AssetWriter...");
        
        NSString *documentsDirectoryPath = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) objectAtIndex:0];
        NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
        [dateFormatter setDateFormat:@"yyyy-MM-dd'T'HH-mm-ss'.mp4'"];
        NSString *outputFileName = [dateFormatter stringFromDate:[NSDate date]];
        outputFilePath = [NSString stringWithFormat:@"%@/%@", documentsDirectoryPath, outputFileName];
        NSData *outputFileData = [outputFileName dataUsingEncoding:NSUTF8StringEncoding];
        memcpy(videoPath, [outputFileData bytes], outputFileData.length);
        NSError *error = nil;
        NSFileManager *fileManager = [NSFileManager defaultManager];
        if ([fileManager fileExistsAtPath:outputFilePath]) {
            [fileManager removeItemAtPath:outputFilePath error:&error];
        }
        
        assetWriter = [AVAssetWriter assetWriterWithURL:[NSURL fileURLWithPath:outputFilePath] fileType:AVFileTypeMPEG4 error:&error];
        
        NSDictionary *videoSettings = @{
                                        AVVideoCodecKey: AVVideoCodecTypeH264,
                                                AVVideoWidthKey: [NSNumber numberWithFloat:size.width],
                                                AVVideoHeightKey: [NSNumber numberWithFloat:size.height]
                                        };
        assetWriterInput = [AVAssetWriterInput assetWriterInputWithMediaType:AVMediaTypeVideo outputSettings:videoSettings];
        [assetWriterInput setExpectsMediaDataInRealTime:YES];
        
        NSDictionary *pixelBufferAttributes = @{
                                                (NSString *)kCVPixelBufferPixelFormatTypeKey: @(kCVPixelFormatType_24RGB),
                                                        (NSString *)kCVPixelBufferWidthKey: [NSNumber numberWithFloat:size.width],
                                                        (NSString *)kCVPixelBufferHeightKey: [NSNumber numberWithFloat:size.height]
                                                };
        adaptor = [AVAssetWriterInputPixelBufferAdaptor assetWriterInputPixelBufferAdaptorWithAssetWriterInput:assetWriterInput
                                                                                   sourcePixelBufferAttributes:pixelBufferAttributes];
        
        [assetWriter addInput:assetWriterInput];
    }

    void initHelper() {
        isRecording = false;
        
        lastTimestamp = kCMTimeZero;
    }

    void startRecording(int width, int height, int x, int y, int w, int h, char *videoPath) {
        NSLog(@"start recording...");
        isRecording = YES;
        
        size = CGSizeMake(width, height);
        frame_x = x;
        frame_y = y;
        frame_w = w;
        frame_h = h;
        setupAssetWriter(videoPath);

        [assetWriter startWriting];
        [assetWriter startSessionAtSourceTime:lastTimestamp];
    }

    void stopRecording() {
        NSLog(@"stop recording...");
        isRecording = NO;
        
        [assetWriterInput markAsFinished];
        [assetWriter finishWritingWithCompletionHandler:^{
            if ([assetWriter status] == AVAssetWriterStatusCompleted) {
                NSLog(@"Successfully save to documents");
                /*
                NSFileManager *fileManager = [NSFileManager defaultManager];
                if ([fileManager fileExistsAtPath:outputFilePath]) {
                    [[PHPhotoLibrary sharedPhotoLibrary] performChanges:^{
                        [PHAssetChangeRequest creationRequestForAssetFromVideoAtFileURL:[NSURL fileURLWithPath:outputFilePath]];
                    } completionHandler:^(BOOL success, NSError * _Nullable error) {
                        if (success) {
                            NSLog(@"save to camera roll");
                        } else {
                            NSLog(@"save fail::%@", error.localizedDescription);
                        }
                    }];
                }
                */
            } else {
                NSLog(@"%@", [assetWriter error]);
            }
        }];
    }

    void processBuffer(int ts, int width, int height, int stride, int *pixelData) {
        char *baseAddress = (char *)pixelData;
        lastTimestamp = CMTimeMake(ts, 600);
        if (isRecording) {
            CVPixelBufferRef pixelBufferOut;

            int span = 3 * sizeof(char);
            NSMutableData *data = [NSMutableData dataWithCapacity:frame_w * frame_h * span];
            for (int i = 0; i< frame_h; i++) {
                [data appendBytes:baseAddress+((frame_x + i) * width + frame_y) * span length:frame_w * span];
            }
            
            CVPixelBufferCreateWithBytes(NULL, frame_w, frame_h, kCVPixelFormatType_24RGB, baseAddress, frame_w * span, NULL, NULL, NULL, &pixelBufferOut);

            if ([adaptor.assetWriterInput isReadyForMoreMediaData]) {
                // [assetWriterInput appendSampleBuffer:sampleBuffer];
                [adaptor appendPixelBuffer:pixelBufferOut withPresentationTime:lastTimestamp];
            } else {
                NSLog(@"writer not ready");
            }
        }
    }

}
