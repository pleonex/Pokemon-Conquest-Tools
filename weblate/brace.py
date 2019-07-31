#   Copyright 2019 Benito Palacios Sanchez (aka pleonex)
#
#   Licensed under the Apache License, Version 2.0 (the "License");
#   you may not use this file except in compliance with the License.
#   You may obtain a copy of the License at
#
#       http://www.apache.org/licenses/LICENSE-2.0
#
#   Unless required by applicable law or agreed to in writing, software
#   distributed under the License is distributed on an "AS IS" BASIS,
#   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
#   See the License for the specific language governing permissions and
#   limitations under the License.
"""Weblate format for brace control code styles."""

from django.utils.translation import ugettext_lazy as _
from weblate.checks.base import TargetCheck
import re

BRACE_MATCH = re.compile(r'{[^}]+}')

class BraceCheck(TargetCheck):
    check_id = 'brace-format'
    name = _('Mismatched brace control codes')
    description = _('Brace control codes in translation does not match source')
    severity = 'warning'
    default_disabled = True

    def check_highlight(self, source, unit):
        if self.should_skip(unit):
            return []
        ret = []
        for match in BRACE_MATCH.finditer(source):
            ret.append((match.start(), match.end(), match.group()))
        return ret

    def check_single(self, source, target, unit):
        src_match = BRACE_MATCH.findall(source)
        if not src_match:
            return False

        tgt_match = BRACE_MATCH.findall(target)
        if len(src_match) != len(tgt_match):
            return True

        return src_match != tgt_match
