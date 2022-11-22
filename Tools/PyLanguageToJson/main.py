import io
import json
import os

dir_path = os.path.dirname(os.path.realpath(__file__))
languageFileLocation = dir_path + '/language/languagefile.txt'


def read_text(path):
    with open(path) as f:
        return f.read()


class Langugage:
    words = []

    def __init__(self, words):
        self.words = words

    def print_words(self):
        return self.words


text = read_text(languageFileLocation)

splitedSpace = text.split('\n')
listOfLans = []

for str in splitedSpace:
    lan = Langugage(str.split(' '))
    print(lan.print_words())
    listOfLans.append(lan.words)

with open("language.json", "w") as write:
    json.dump(listOfLans, write, ensure_ascii=False)
