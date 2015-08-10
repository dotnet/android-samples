/*
 * Copyright (C) 2014 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

(function () {
    "use strict";

    navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia;
    window.URL = window.URL || window.webkitURL;

    window.onload = function () {

        var video = document.querySelector('#video'),
            toggle = document.querySelector('#toggle'),
            stream = null;

        if (!navigator.getUserMedia) {
            console.error('getUserMedia not supported');
        }

        toggle.addEventListener('click', function () {
            if (null === stream) {
                // This call to "getUserMedia" initiates a PermissionRequest in the WebView.
                navigator.getUserMedia({ video: true }, function (s) {
                    stream = s;
                    video.src = window.URL.createObjectURL(stream);
                    toggle.innerText = 'Stop';
                    console.log('Started');
                }, function (error) {
                    console.error('Error starting camera. Denied.');
                });
            } else {
                stream.stop();
                stream = null;
                toggle.innerText = 'Start';
                console.log('Stopped');
            }
        });

        console.log('Page loaded');

    };

})();
