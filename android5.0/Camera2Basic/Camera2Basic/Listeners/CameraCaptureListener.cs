
using Android.Hardware.Camera2;
using Java.IO;
using Java.Lang;

namespace Camera2Basic.Listeners
{
    public class CameraCaptureListener : CameraCaptureSession.CaptureCallback
    {
        public Camera2BasicFragment Owner { get; set; }
        public File File { get; set; }
        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            Process(result);
        }

        public override void OnCaptureProgressed(CameraCaptureSession session, CaptureRequest request, CaptureResult partialResult)
        {
            Process(partialResult);
        }

        private void Process(CaptureResult result)
        {
            switch (Owner.mState)
            {
                case Camera2BasicFragment.STATE_WAITING_LOCK:
                    {
                        Integer afState = (Integer)result.Get(CaptureResult.ControlAfState);
                        if (afState == null)
                        {
                            Owner.CaptureStillPicture();
                        }

                        else if ((((int)ControlAFState.FocusedLocked) == afState.IntValue()) ||
                                   (((int)ControlAFState.NotFocusedLocked) == afState.IntValue()))
                        {
                            // ControlAeState can be null on some devices
                            Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                            if (aeState == null ||
                                    aeState.IntValue() == ((int)ControlAEState.Converged))
                            {
                                Owner.mState = Camera2BasicFragment.STATE_PICTURE_TAKEN;
                                Owner.CaptureStillPicture();
                            }
                            else
                            {
                                Owner.RunPrecaptureSequence();
                            }
                        }
                        break;
                    }
                case Camera2BasicFragment.STATE_WAITING_PRECAPTURE:
                    {
                        // ControlAeState can be null on some devices
                        Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                        if (aeState == null ||
                                aeState.IntValue() == ((int)ControlAEState.Precapture) ||
                                aeState.IntValue() == ((int)ControlAEState.FlashRequired))
                        {
                            Owner.mState = Camera2BasicFragment.STATE_WAITING_NON_PRECAPTURE;
                        }
                        break;
                    }
                case Camera2BasicFragment.STATE_WAITING_NON_PRECAPTURE:
                    {
                        // ControlAeState can be null on some devices
                        Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                        if (aeState == null || aeState.IntValue() != ((int)ControlAEState.Precapture))
                        {
                            Owner.mState = Camera2BasicFragment.STATE_PICTURE_TAKEN;
                            Owner.CaptureStillPicture();
                        }
                        break;
                    }
            }
        }
    }
}