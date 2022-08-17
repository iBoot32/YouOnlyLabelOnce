# YouOnlyLabelOnce
A server+client designed to allow easy team collaboration in YOLOv5 Image Labeling


# To Use:

Download the server, and create three directories:

- unannotated
- annotated
- annotations

Place any unannotated images in the unannotated folder. Upon annotation via the client program, the images will be moved to `annotated` and given an annotation file in `annotations`.

# Using the GUI Client:

Open the client and connect to the labeling server. You should have an image displayed ready for labeling. 

To annotate the image, simply select either Red/Blue (depending on the color of the armor you're labeling) and click where you would like to add one corner of a bounding box, then click where you'd like the opposite corner to be.
Once satisfied, send the annotation to the server.
