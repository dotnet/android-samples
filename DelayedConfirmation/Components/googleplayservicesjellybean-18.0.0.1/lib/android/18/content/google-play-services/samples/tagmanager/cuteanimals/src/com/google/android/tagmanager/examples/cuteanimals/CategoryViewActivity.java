package com.google.android.tagmanager.examples.cuteanimals;

import android.app.Activity;
import android.content.Intent;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.text.Html;
import android.view.View;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.google.android.gms.tagmanager.TagManager;

/**
 * An {@link Activity} that displays a list of image files for one category; clicking on one opens
 * an {@link ImageViewActivity} to display the actual image.
 * <p>
 * This activity is invoked by {@link MainActivity} which is excepted to pass in the category name
 * and image file name array associated with category.
 */
public class CategoryViewActivity extends Activity {
    // The key of the category_name to be passed in.
    static final String CATEGORY_NAME_KEY = "category_name";
    // The key of the image file name array to be passed in.
    static final String IMAGE_FILES_KEY = "image_files";
    private String adjective;
    private String categoryName;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_category_view);

        // Retrieve the passed in information.
        Bundle extras = getIntent().getExtras();
        if (extras == null) {
            return;
        }

        adjective = ContainerHolderSingleton.getContainerHolder().getContainer().
                getString(MainActivity.ADJECTIVE_KEY);
        categoryName = extras.getString(CATEGORY_NAME_KEY);

        // Set the text for the back to MainActivity button.
        Button backButton = (Button) findViewById(R.id.back_to_main);
        backButton.setText(" << " + getDisplayName(getResources().getString(R.string.animals)));

        // Set the text of the title.
        String title = getDisplayName(categoryName) + " " + getResources().getString(R.string.images);
        ((TextView) findViewById(R.id.category_view_title)).setText(title);

        // Dynamically insert a button for each image file.
        String[] fileNames = extras.getStringArray(IMAGE_FILES_KEY);
        LinearLayout layout = (LinearLayout) findViewById(R.id.image_files_linear_layout);
        for (final String fileName : fileNames) {
            Button button = createImageViewButton(fileName);
            layout.addView(button);
        }

        // Put the category_name into the data layer for future use.
        TagManager.getInstance(this).getDataLayer().push(CATEGORY_NAME_KEY, categoryName);
    }

    @Override
    protected void onStart() {
        super.onStart();
        Utils.pushOpenScreenEvent(this, "CategoryViewScreen");
    }

    @Override
    protected void onStop() {
        super.onStop();
        Utils.pushCloseScreenEvent(this, "CategoryViewScreen");
    }

    public void backToMainButtonClicked(@SuppressWarnings("unused") View view) {
        // Back to previous activity.
        finish();
    }

    private Button createImageViewButton(final String fileName) {
        Button button = new Button(this);
        // Set the text of the button, the first line is the image file name, the second one
        // is to show size of the image.
        String firstLineHtmlText = fileName;
        int imageId = getResources().getIdentifier(fileName, "drawable", getPackageName());
        Drawable drawable = getResources().getDrawable(imageId);
        String secondLineHtmlText = "<small><font color='grey'>"
                + drawable.getMinimumWidth() + "x" + drawable.getMinimumHeight() + "</font></small>";
        button.setText(Html.fromHtml(firstLineHtmlText + "<br/>" + secondLineHtmlText));
        button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View arg0) {
                startImageViewActivity(fileName);
            }
        });

        return button;
    }

    private void startImageViewActivity(String fileName) {
        Intent intent = new Intent(CategoryViewActivity.this, ImageViewActivity.class);
        // Passes the image file name and the back button name into the ImageViewActivity.
        intent.putExtra(ImageViewActivity.IMAGE_NAME_KEY, fileName);
        intent.putExtra(ImageViewActivity.BACK_BUTTON_NAME_KEY, getDisplayName(categoryName));
        startActivity(intent);
    }

    private String getDisplayName(String name) {
        return adjective + " " + name;
    }
}
