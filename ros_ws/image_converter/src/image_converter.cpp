#include <ros/ros.h>
#include <image_transport/image_transport.h>
#include <cv_bridge/cv_bridge.h>
#include <sensor_msgs/image_encodings.h>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/highgui/highgui.hpp>

static const std::string OPENCV_WINDOW = "Image window";

class ImageConverter
{
  ros::NodeHandle nh_;
  image_transport::ImageTransport it_;
  image_transport::Subscriber image_sub_;
  image_transport::Publisher image_pub_;
  
  public:
    ImageConverter()
      : it_(nh_)
    {
      // Subscrive to input video feed and publish output video feed
      image_sub_ = it_.subscribe("/camera/depth_registered/image_raw", 1, 
        &ImageConverter::imageCb, this);
      image_pub_ = it_.advertise("/image_converter/output_video", 1);

      cv::namedWindow(OPENCV_WINDOW);
    }

    ~ImageConverter()
    {
      cv::destroyWindow(OPENCV_WINDOW);
    }

    
  void imageCb(const sensor_msgs::ImageConstPtr& msg)
  {
    namespace enc = sensor_msgs::image_encodings;
    cv_bridge::CvImageConstPtr cv_ptr;
    cv::Mat conversion_mat_;
    cv::Mat img_scaled_8u;        

    
    try
    {
      cv_ptr = cv_bridge::toCvShare(msg);//now cv_ptr is the matrix, do not forget "TYPE_" before "16UC1"

      if (msg->encoding == "16UC1" || msg->encoding == "32FC1") {
        // scale / quantify
        double min = 0;
        double max = 3.5;
        if (msg->encoding == "16UC1") max *= 1000;

        //cv::minMaxLoc(cv_ptr->image, &min, &max);

        //ROS_ERROR("max: %f", max);

        cv::Mat(cv_ptr->image).convertTo(img_scaled_8u, CV_8UC1, 255. / (max - min));
        //cv::cvtColor(img_scaled_8u, conversion_mat_, CV_GRAY2RGB);
      }
    }
    catch (cv_bridge::Exception& e)
    {
      ROS_ERROR("cv_bridge exception: %s", e.what());
      return;
    }

    // Process cv_ptr->image using OpenCV
    cv_bridge::CvImage out_msg;
    out_msg.header   = msg->header; // Same timestamp and tf frame as input image
    out_msg.encoding = sensor_msgs::image_encodings::MONO8; // Or whatever
    out_msg.image    = img_scaled_8u; // Your cv::Mat

    image_pub_.publish(out_msg.toImageMsg());

    //image_pub_.publish(cv_ptr->toImageMsg());

  // image must be copied since it uses the conversion_mat_ for storage which is asynchronously overwritten in the next callback invocation
  //QImage image(conversion_mat_.data, conversion_mat_.cols, conversion_mat_.rows, conversion_mat_.step[0], QImage::Format_RGB888);
  //ui_.image_frame->setImage(image);
  }
};

int main(int argc, char** argv)
{
  ros::init(argc, argv, "image_converter");
  ImageConverter ic;
  ros::spin();
  return 0;
}
