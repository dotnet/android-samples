package com.google.android.tagmanager.examples.cuteanimals;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.json.JSONArray;
import org.json.JSONObject;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.StrictMode;
import android.text.Html;
import android.util.Log;
import android.view.Menu;
import android.view.View;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.google.android.gms.tagmanager.Container;
import com.google.android.gms.tagmanager.TagManager;

/**
 * An {@link Activity} that displays a list of animal categories; clicking on one opens a
 * {@link CategoryViewActivity} to display a list of image files associated with this category.
 */
public class MainActivity extends Activity {
    static final String TAG = "GTMExample";
    static final String ADJECTIVE_KEY = "adjective";
    private static final String CATEGORY_KEY = "category";

    // Set to false for release build.
    private static final Boolean DEVELOPER_BUILD = true;
    private Container container;
    private String adjective;
    // Key is the animal category name, and the value is a list of image file names.
    private Map<String, List<String>> categoryImagesMap = new HashMap<String, List<String>>();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        if (DEVELOPER_BUILD) {
            StrictMode.enableDefaults();
        }
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_main);

        container = ContainerHolderSingleton.getContainerHolder().getContainer();
        updateCategories();
    }

    @Override
    protected void onStart() {
        super.onStart();
        Utils.pushOpenScreenEvent(this, "MainScreen");
    }

    @Override
    protected void onStop() {
        super.onStop();
        Utils.pushCloseScreenEvent(this, "MainScreen");
    }

    private void updateCategories() {
        adjective = container.getString(ADJECTIVE_KEY);
        // Update the title.
        TextView titleView = (TextView) findViewById(R.id.title);
        titleView.setText(getDisplayName(getResources().getString(R.string.animals)));

        LinearLayout linearLayout = (LinearLayout) findViewById(R.id.category_linear_layout);
        linearLayout.removeAllViews();

        // Retrieve the animal categories information from GTM.
        // The Json file to store the default categories information
        // is at assets/tagmanager/GTM-XXXX.json.
        String categoriesJsonString = container.getString(CATEGORY_KEY);
        // A list to save all animal category names, such as bunny, cat and etc.
        List<String> categoryNames = new ArrayList<String>();
        categoryImagesMap.clear();

        // No category information returned from container.
        if (categoriesJsonString.isEmpty()) {
            TextView message = new TextView(this);
            message.setText("No animal category found.");
            linearLayout.addView(message);
            return;
        }

        try {
            // GTM doesn't support returning compound objects, so we store the category map as a JSON
            // string which we then parse.
            //
            // Here is a sample JSON string returned back from container:
            //
            // '[
            //     {"name": "Bunny", "image_files": ["bunny_1", "bunny_2", "bunny_3"]},
            //     {"name": "Tiger", "image_files": ["tiger_1", "tiger_2"]}
            // }]'
            JSONArray categories = new JSONArray(categoriesJsonString);
            for (int i = 0; i < categories.length(); i++) {
                JSONObject category = categories.getJSONObject(i);
                String categoryName = category.getString("name");
                JSONArray images = category.getJSONArray("image_files");
                categoryNames.add(categoryName);
                List<String> imageNames = new ArrayList<String>(images.length());
                for (int j = 0; j < images.length(); j++) {
                    imageNames.add(images.getString(j));
                }
                categoryImagesMap.put(categoryName, imageNames);
            }
        } catch (Exception e) {
            Log.e(TAG, "Parsing the JSON string: [" + categoriesJsonString + "] throw an exception.", e);
            return;
        }

        // Dynamically insert a button for each category.
        for (String categoryName : categoryNames) {
            Button button = createButton(categoryName);
            linearLayout.addView(button);
        }
    }

    public void refreshButtonClicked(@SuppressWarnings("unused") View view) {
        Log.i(TAG, "refreshButtonClicked");
        ContainerHolderSingleton.getContainerHolder().refresh();

        // Push the "refresh" event to trigger firing an analytics tag.
        TagManager.getInstance(this).getDataLayer().push("event", "refresh");
        // Push the "custom tag" event to trigger firing a custom function call tag.
        TagManager.getInstance(this).getDataLayer().push("event", "custom_tag");
        updateCategories();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.activity_main, menu);
        return true;
    }

    private Button createButton(final String categoryName) {
        Button button = new Button(this);
        // Set the text of the button, the first line is the category name, the second one
        // is to show the number of images for this category.
        String firstLineHtmlText = getDisplayName(categoryName) + " "
                + getResources().getString(R.string.pictures);
        String secondLineHtmlText = "<small><font color='grey'>"
                + categoryImagesMap.get(categoryName).size() + " "
                + getResources().getString(R.string.images) + "</font></small>";
        button.setText(Html.fromHtml(firstLineHtmlText + "<br/>" + secondLineHtmlText));

        button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View arg0) {
                startCategoryViewActivity(categoryName);
            }
        });

        return button;
    }

    private void startCategoryViewActivity(String categoryName) {
        Intent intent = new Intent(MainActivity.this, CategoryViewActivity.class);
        // Passes the category name and the image file name array into CategoryViewActivity.
        intent.putExtra(CategoryViewActivity.CATEGORY_NAME_KEY, categoryName);
        intent.putExtra(CategoryViewActivity.IMAGE_FILES_KEY,
                categoryImagesMap.get(categoryName).toArray(new String[1]));
        startActivity(intent);
    }

    private String getDisplayName(String name) {
        return adjective + " " + name;
    }
}
