# Speech Translator quick-start

## Overview

In the context of a customer project, I used the Speech Translator quick-start that is available here: [Recognize and translate speech to text](https://learn.microsoft.com/en-us/azure/ai-services/speech-service/get-started-speech-translation).

This code is a straight forward project of the code from the article.

It requires the creation of a Speech Service instance in Azure:

  ![Azure Speech](https://github.com/embergershared/share-as-you-go/blob/main/ai/anouncement/media/2023-08-17_123132.png)

## Instructions

1. Create the Speech service instance in Azure

2. Get the `Key` and `Location/Region` values of the Speech service

3. Create on the machine that will run the console app these 2 environment variables:

    - `SPEECH_KEY` with the value of Azure Speech service `Key`
    - `SPEECH_REGION` with the value of Azure Speech service `Location/Region`

4. Launch the console app

5. When seeing this screen, talk in the `en-US` language (can be changed in line `45` of `program.cs`)

    ![Prompt](https://github.com/embergershared/share-as-you-go/blob/main/ai/anouncement/media/2023-08-17_124449.png)

6. You'll get the translated text back in `es` (espanol) (can be changed in line `46` of `program.cs`)

    ![Result](https://github.com/embergershared/share-as-you-go/blob/main/ai/anouncement/media/2023-08-17_124609.png)

## Additional considerations

As the Speech service is hosted in Azure, it comes with all the Azure security features like:

- `VNet` connectivity
- `Private endpoints`
- `Managed identity`
- `Encryption with Customer Managed Keys`

It can easily be integrated in a secure solution, for example, one that would run this code in a container hosted in an [Azure Kubernetes Service (AKS)](https://azure.microsoft.com/en-us/products/kubernetes-service).
