/*
 * Copyright (C) 2013 The Android Open Source Project
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

package android.support.v4.text;

import android.test.AndroidTestCase;

import java.util.Locale;

/** @hide */
public class BidiFormatterTest extends AndroidTestCase {

    private static final BidiFormatter LTR_FMT = BidiFormatter.getInstance(false /* LTR context */);
    private static final BidiFormatter RTL_FMT = BidiFormatter.getInstance(true /* RTL context */);

    private static final BidiFormatter LTR_FMT_EXIT_RESET =
            new BidiFormatter.Builder(false /* LTR context */).stereoReset(false).build();
    private static final BidiFormatter RTL_FMT_EXIT_RESET =
            new BidiFormatter.Builder(true /* RTL context */).stereoReset(false).build();

    private static final String EN = "abba";
    private static final String HE = "\u05e0\u05e1";

    private static final String LRM = "\u200E";
    private static final String RLM = "\u200F";
    private static final String LRE = "\u202A";
    private static final String RLE = "\u202B";
    private static final String PDF = "\u202C";

    public void testIsRtlContext() {
        assertEquals(false, LTR_FMT.isRtlContext());
        assertEquals(true, RTL_FMT.isRtlContext());

        assertEquals(false, BidiFormatter.getInstance(Locale.ENGLISH).isRtlContext());
        assertEquals(true, BidiFormatter.getInstance(true).isRtlContext());
    }

    public void testBuilderIsRtlContext() {
        assertEquals(false, new BidiFormatter.Builder(false).build().isRtlContext());
        assertEquals(true, new BidiFormatter.Builder(true).build().isRtlContext());
    }

    public void testIsRtl() {
        assertEquals(true, BidiFormatter.getInstance(true).isRtl(HE));
        assertEquals(true, BidiFormatter.getInstance(false).isRtl(HE));

        assertEquals(false, BidiFormatter.getInstance(true).isRtl(EN));
        assertEquals(false, BidiFormatter.getInstance(false).isRtl(EN));
    }

    public void testUnicodeWrap() {
        // Uniform directionality in opposite context.
        assertEquals("uniform dir opposite to LTR context",
                RLE + "." + HE + "." + PDF + LRM,
                LTR_FMT_EXIT_RESET.unicodeWrap("." + HE + "."));
        assertEquals("uniform dir opposite to LTR context, stereo reset",
                LRM + RLE + "." + HE + "." + PDF + LRM,
                LTR_FMT.unicodeWrap("." + HE + "."));
        assertEquals("uniform dir opposite to LTR context, stereo reset, no isolation",
                RLE + "." + HE + "." + PDF,
                LTR_FMT.unicodeWrap("." + HE + ".", false));
        assertEquals("neutral treated as opposite to LTR context",
                RLE + "." + PDF + LRM,
                LTR_FMT_EXIT_RESET.unicodeWrap(".", TextDirectionHeuristicsCompat.RTL));
        assertEquals("uniform dir opposite to RTL context",
                LRE + "." + EN + "." + PDF + RLM,
                RTL_FMT_EXIT_RESET.unicodeWrap("." + EN + "."));
        assertEquals("uniform dir opposite to RTL context, stereo reset",
                RLM + LRE + "." + EN + "." + PDF + RLM,
                RTL_FMT.unicodeWrap("." + EN + "."));
        assertEquals("uniform dir opposite to RTL context, stereo reset, no isolation",
                LRE + "." + EN + "." + PDF,
                RTL_FMT.unicodeWrap("." + EN + ".", false));
        assertEquals("neutral treated as opposite to RTL context",
                LRE + "." + PDF + RLM,
                RTL_FMT_EXIT_RESET.unicodeWrap(".", TextDirectionHeuristicsCompat.LTR));

        // We test mixed-directionality cases only with an explicit overall directionality parameter
        // because the estimation logic is outside the sphere of BidiFormatter, and different
        // estimators will treat them differently.

        // Overall directionality matching context, but with opposite exit directionality.
        assertEquals("exit dir opposite to LTR context",
                EN + HE + LRM,
                LTR_FMT_EXIT_RESET.unicodeWrap(EN + HE, TextDirectionHeuristicsCompat.LTR));
        assertEquals("exit dir opposite to LTR context, stereo reset",
                EN + HE + LRM,
                LTR_FMT.unicodeWrap(EN + HE, TextDirectionHeuristicsCompat.LTR));
        assertEquals("exit dir opposite to LTR context, stereo reset, no isolation",
                EN + HE,
                LTR_FMT.unicodeWrap(EN + HE, TextDirectionHeuristicsCompat.LTR, false));

        assertEquals("exit dir opposite to RTL context",
                HE + EN + RLM,
                RTL_FMT_EXIT_RESET.unicodeWrap(HE + EN, TextDirectionHeuristicsCompat.RTL));
        assertEquals("exit dir opposite to RTL context, stereo reset",
                HE + EN + RLM,
                RTL_FMT.unicodeWrap(HE + EN, TextDirectionHeuristicsCompat.RTL));
        assertEquals("exit dir opposite to RTL context, stereo reset, no isolation",
                HE + EN,
                RTL_FMT.unicodeWrap(HE + EN, TextDirectionHeuristicsCompat.RTL, false));

        // Overall directionality matching context, but with opposite entry directionality.
        assertEquals("entry dir opposite to LTR context",
                HE + EN,
                LTR_FMT_EXIT_RESET.unicodeWrap(HE + EN, TextDirectionHeuristicsCompat.LTR));
        assertEquals("entry dir opposite to LTR context, stereo reset",
                LRM + HE + EN,
                LTR_FMT.unicodeWrap(HE + EN, TextDirectionHeuristicsCompat.LTR));
        assertEquals("entry dir opposite to LTR context, stereo reset, no isolation",
                HE + EN,
                LTR_FMT.unicodeWrap(HE + EN, TextDirectionHeuristicsCompat.LTR, false));

        assertEquals("entry dir opposite to RTL context",
                EN + HE,
                RTL_FMT_EXIT_RESET.unicodeWrap(EN + HE, TextDirectionHeuristicsCompat.RTL));
        assertEquals("entry dir opposite to RTL context, stereo reset",
                RLM + EN + HE,
                RTL_FMT.unicodeWrap(EN + HE, TextDirectionHeuristicsCompat.RTL));
        assertEquals("entry dir opposite to RTL context, stereo reset, no isolation",
                EN + HE,
                RTL_FMT.unicodeWrap(EN + HE, TextDirectionHeuristicsCompat.RTL, false));

        // Overall directionality matching context, but with opposite entry and exit directionality.
        assertEquals("entry and exit dir opposite to LTR context",
                HE + EN + HE + LRM,
                LTR_FMT_EXIT_RESET.unicodeWrap(HE + EN + HE, TextDirectionHeuristicsCompat.LTR));
        assertEquals("entry and exit dir opposite to LTR context, stereo reset",
                LRM + HE + EN + HE + LRM,
                LTR_FMT.unicodeWrap(HE + EN + HE, TextDirectionHeuristicsCompat.LTR));
        assertEquals("entry and exit dir opposite to LTR context, no isolation",
                HE + EN + HE,
                LTR_FMT_EXIT_RESET.unicodeWrap(HE + EN + HE, TextDirectionHeuristicsCompat.LTR, false));

        assertEquals("entry and exit dir opposite to RTL context",
                EN + HE + EN + RLM,
                RTL_FMT_EXIT_RESET.unicodeWrap(EN + HE + EN, TextDirectionHeuristicsCompat.RTL));
        assertEquals("entry and exit dir opposite to RTL context, no isolation",
                EN + HE + EN,
                RTL_FMT_EXIT_RESET.unicodeWrap(EN + HE + EN, TextDirectionHeuristicsCompat.RTL, false));

        // Entry and exit directionality matching context, but with opposite overall directionality.
        assertEquals("overall dir (but not entry or exit dir) opposite to LTR context",
                RLE + EN + HE + EN + PDF + LRM,
                LTR_FMT_EXIT_RESET.unicodeWrap(EN + HE + EN, TextDirectionHeuristicsCompat.RTL));
        assertEquals("overall dir (but not entry or exit dir) opposite to LTR context, stereo reset",
                LRM + RLE + EN + HE + EN + PDF + LRM,
                LTR_FMT.unicodeWrap(EN + HE + EN, TextDirectionHeuristicsCompat.RTL));
        assertEquals("overall dir (but not entry or exit dir) opposite to LTR context, no isolation",
                RLE + EN + HE + EN + PDF,
                LTR_FMT_EXIT_RESET.unicodeWrap(EN + HE + EN, TextDirectionHeuristicsCompat.RTL, false));

        assertEquals("overall dir (but not entry or exit dir) opposite to RTL context",
                LRE + HE + EN + HE + PDF + RLM,
                RTL_FMT_EXIT_RESET.unicodeWrap(HE + EN + HE, TextDirectionHeuristicsCompat.LTR));
        assertEquals("overall dir (but not entry or exit dir) opposite to RTL context, stereo reset",
                RLM + LRE + HE + EN + HE + PDF + RLM,
                RTL_FMT.unicodeWrap(HE + EN + HE, TextDirectionHeuristicsCompat.LTR));
        assertEquals("overall dir (but not entry or exit dir) opposite to RTL context, no isolation",
                LRE + HE + EN + HE + PDF,
                RTL_FMT_EXIT_RESET.unicodeWrap(HE + EN + HE, TextDirectionHeuristicsCompat.LTR, false));
    }
}
