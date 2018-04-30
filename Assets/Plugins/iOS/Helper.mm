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
        NSData *outputFileData = [outputFilePath dataUsingEncoding:NSASCIIStringEncoding allowLossyConversion:YES];
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

    void startRecording(int width, int height, char *videoPath) {
        NSLog(@"start recording...");
        isRecording = YES;
        
        size = CGSizeMake(width, height);
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
        lastTimestamp = CMTimeMake(ts, 600);
        if (isRecording) {
            CVPixelBufferRef pixelBufferOut;

    //        NSMutableData *data = [NSMutableData dataWithCapacity:size.width*size.height*sizeof(int)];
    //
    //        size_t r = (height-size.height)/2;
    //        size_t c = (width-size.width)/2;
    //        size_t stride = size.width*sizeof(int);
    //        for (int i = 0; i<size.height; i++) {
    //            [data appendBytes:baseAddress+(r+i)*width+c length:stride];
    //        }
            
            CVPixelBufferCreateWithBytes(NULL, width, height, kCVPixelFormatType_24RGB, pixelData, stride, NULL, NULL, NULL, &pixelBufferOut);

            if ([adaptor.assetWriterInput isReadyForMoreMediaData]) {
                // [assetWriterInput appendSampleBuffer:sampleBuffer];
                [adaptor appendPixelBuffer:pixelBufferOut withPresentationTime:lastTimestamp];
            } else {
                NSLog(@"writer not ready");
            }
        }
    }

}
