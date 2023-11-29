# ZetiWeb

Acknowledgements:

I would have added more appropriate tests i.e. testing services ect.. and string sanitization permutations.
I covered most happy paths and should have added constants and repeated strings in a constants file but for times sake I appreciate its not 
a perfect solution. Also there was quite alot of code in the controller. I could have moved some of the code into services expecially where there was logic and added unit tests.

This task would be better suited to a stateless solution such as a durable function app to run in parellel request and an initial payload.
