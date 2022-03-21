

from typing import Tuple
import pygame


class Text:
    def __init__(self, font: str, fontSize: int, txt: str, color: Tuple, pos):
        self.font = pygame.font.Font(font, fontSize)
        self.txt = font.render(txt, True, color)