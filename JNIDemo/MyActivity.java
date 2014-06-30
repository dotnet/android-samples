package mono.samples.jnitest;

import android.os.Bundle;
import android.app.Activity;
import android.widget.*;
import android.view.View.*;
import android.view.View;

public class MyActivity extends Activity implements OnClickListener {
	
	Button helloButton;
	TextView text;
	int count = 0;
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.helloworld);
        helloButton = (Button)findViewById(R.id.helloButton);
        helloButton.setOnClickListener(this);

        text = (TextView)findViewById(R.id.helloText);

    }

	@Override
	public void onClick(View v)
	{
		count ++;
		text.setText("You've said hello " + count + " times");
	}
}
