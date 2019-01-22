//
//  LUCVarEditController.h
//
//  Lunar Unity Mobile Console
//  https://github.com/SpaceMadness/lunar-unity-console
//
//  Copyright 2018 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

#import "LUViewController.h"

@class LUCVar;
@class LUCVarEditController;

@protocol LUCVarEditControllerDelegate <NSObject>

- (void)editController:(LUCVarEditController *)controller didChangeValue:(NSString *)value;

@end

@interface LUCVarEditController : LUViewController

@property (nonatomic, weak) id<LUCVarEditControllerDelegate> delegate;

- (instancetype)initWithVariable:(LUCVar *)variable;

@end
