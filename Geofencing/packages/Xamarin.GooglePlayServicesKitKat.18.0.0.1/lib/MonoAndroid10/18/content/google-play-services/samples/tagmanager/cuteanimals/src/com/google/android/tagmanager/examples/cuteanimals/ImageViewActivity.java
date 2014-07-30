package com.google.android.tagmanager.examples.cuteanimals;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

import com.google.android.gms.tagmanager.TagManager;

/**
 * Activity to view an image.
 * <p>
 * This activity is invoked by {@link CategoryViewActivity} which is expected to pass in
 * the image file name and the back button name.
 */
public class ImageViewActivity extends Activity {
    // The key of the back button name to be passed in.
    static final String BACK_BUTTON_NAME_KEY = "back_button_name";
    // The key of the image file name to be passed in.
    static final String IMAGE_NAME_KEY = "image_name";
    private String imageName;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_image_view);

        Bundle extras = getIntent().getExtras();
        if (extras == null) {
            return;
        }

        imageName = extras.getString(IMAGE_NAME_KEY);

        // Modify the text for the backToCategory button.
        Button button = (Button) findViewById(R.id.back_to_category);
        button.setText(" << " + extras.getString(BACK_BUTTON_NAME_KEY));
        button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Back to previous activity.
                finish();
            }
        });

        // Set the text of the title.
        TextView titleView = (TextView) findViewById(R.id.image_view_title);
        titleView.setText(imageName);

        // Draw the image.
        int imageId = getResources().getIdentifier(imageName, "drawable", getPackageName());
        ImageView imageView = (ImageView) findViewById(R.id.animal_image);
        imageView.setImageDrawable(getResources().getDrawable(imageId));
        imageView.setContentDescription(imageName);

        // Put the image_name into the data layer for future use.
        TagManager.getInstance(this).getDataLayer().push(IMAGE_NAME_KEY, imageName);
    }

    @Override
    protected void onStart() {
        super.onStart();
        Utils.pushOpenScreenEvent(this, "ImageViewScreen");
    }

    @Override
    protected void onStop() {
        super.onStop();
        Utils.pushCloseScreenEvent(this, "ImageViewScreen");
    }
}
