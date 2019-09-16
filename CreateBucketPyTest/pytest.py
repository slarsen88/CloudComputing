# Create bucket and send object to bucket

import boto3
import logging
from botocore.exceptions import ClientError


def put_object(dest_bucket_name, dest_object_name, src_data):
    if isinstance(src_data, bytes):
        object_data = src_data
    elif isinstance(src_data, str):
        try:
            object_data = open(src_data, 'rb')
        except Exception as e:
            logging.error(e)
            return False
    else:
        logging.error('Type of ' + str(type(src_data)) + ' for the argument \'src_data\' is not supported.')
        return False

    s3 = boto3.client('s3')
    try:
        s3.put_object(Bucket = dest_bucket_name, Key = dest_object_name, Body = object_data)
    except ClientError as e:
        logging.error(e)
        return False
    finally:
        if isinstance(src_data, str):
            object_data.close()
        return True

def makeBucket():
    s3 = boto3.client('s3')
    s3.create_bucket(Bucket='1cc3b04a-f041-41c0-a8bb-3ce0d4ae2e2d')

def main():
    test_bucket_name = '1cc3b04a-f041-41c0-a8bb-3ce0d4ae2e2d'
    test_object_name = 'Object_name.mp4'
    filename = 'C:\\Users\\stuar\\Desktop\\Cloud\\Project3\\last_ding.mp4'
    
    logging.basicConfig(level = logging.DEBUG,
                        format = '%(levelname)s: %(asctime)s: %(message)s')

    success = put_object(test_bucket_name, test_object_name, filename)
    if success:
        logging.info(f'Added {test_object_name} to {test_bucket_name}')

if __name__ == '__main__':
    main()