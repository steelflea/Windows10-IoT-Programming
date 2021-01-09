#pragma once

#include <opencv2\core.hpp>
#include <collection.h>
#include <vector>

using namespace Platform::Collections;
using namespace std;
using namespace Windows::Foundation::Collections;

namespace ImageProcessingComponent {

	public ref class ObjectDescriptor sealed
	{
	public:
		ObjectDescriptor();

		property IVector<Windows::Foundation::Point>^ Points
		{
			IVector<Windows::Foundation::Point>^ get()
			{
				return points;
			}
		}

		property double Area
		{
			double get()
			{
				return area;
			}
		}

	internal:
		ObjectDescriptor(vector<cv::Point> contour, double area);

	private:
		IVector<Windows::Foundation::Point>^ points;
		double area;
	};
}
